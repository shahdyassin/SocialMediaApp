using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data.Dtos;
using SocialMediaApp.Data.Helpers.Constants;
using SocialMediaApp.Data.Models;

namespace SocialMediaApp.Data.Services;

public class FriendsService : IFriendsService
{
    private readonly AppDbContext _context;

    public FriendsService(AppDbContext context)
    {
        _context = context;
    }


    public async Task SendRequestAsync(int senderId, int receiverId)
    {
        var request = new FriendRequest
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Status = FriendShipStatus.Pending,
            DateCreated = DateTime.Now,
            DateUpdated = DateTime.Now
        };
        _context.FriendRequests.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRequestAsync(int requestId, string newStatus)
    {
        var requestDb = await _context.FriendRequests.FirstOrDefaultAsync(n => n.Id == requestId);
        if (requestDb != null)
        {
            requestDb.Status = newStatus;
            requestDb.DateUpdated = DateTime.UtcNow;
            _context.Update(requestDb);
            await _context.SaveChangesAsync();
        }

        if (newStatus == FriendShipStatus.Accepted)
        {
            var friendship = new Friendship
            {
                SenderId = requestDb.SenderId,
                ReceiverId = requestDb.ReceiverId,
                DateCreated = DateTime.UtcNow
            };
            await _context.Friendships.AddAsync(friendship);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFriendAsync(int frienshipId)
    {
        var friendship = await _context.Friendships.FirstOrDefaultAsync(n => n.Id == frienshipId);
        if (friendship != null)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            
            //find requests
            var requests = await _context.FriendRequests
                .Where(r => (r.SenderId == friendship.SenderId && r.ReceiverId == friendship.ReceiverId) ||
                            (r.SenderId == friendship.ReceiverId && r.ReceiverId == friendship.SenderId))
                .ToListAsync();

            if (requests.Any())
            {
                _context.FriendRequests.RemoveRange(requests);
                await _context.SaveChangesAsync();
            }
        }
        
    }

    public async Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId)
    {
        var existingFriendsIds = await _context.Friendships
            .Where(n => n.SenderId == userId && n.ReceiverId == userId)
            .Select(n => n.SenderId == userId ? n.ReceiverId : n.SenderId)
            .ToListAsync();

        //pending requests
        var pendingRequestsIds = await _context.FriendRequests
            .Where(n => (n.SenderId == userId || n.ReceiverId == userId) && n.Status == FriendShipStatus.Pending)
            .Select(n => n.SenderId == userId ? n.ReceiverId : n.SenderId)
            .ToListAsync();

        //Get Suggested Friends 
        var suggestedFriends = await _context.Users
            .Where(n => n.Id != userId &&
                        !existingFriendsIds.Contains(n.Id) &&
                        !pendingRequestsIds.Contains(n.Id))
            .Select(u => new UserWithFriendsCountDto()
            {
                User = u,
                FriendsCount = _context.Friendships
                    .Count(f => f.SenderId == u.Id || f.ReceiverId == u.Id)
            })
            .Take(5)
            .ToListAsync();

        return suggestedFriends;
    }

    public async Task<List<FriendRequest>> GetSentFriendRequestAsync(int userId)
    {
        var friendRequestsSent = await _context.FriendRequests
            .Include(n => n.Sender)
            .Include(n => n.Receiver)
            .Where(f => f.SenderId == userId && f.Status == FriendShipStatus.Pending)
            .ToListAsync();

        return friendRequestsSent;
    }

    public async Task<List<FriendRequest>> GetReceivedFriendRequestAsync(int userId)
    {
        var friendRequestsSent = await _context.FriendRequests
            .Include(n => n.Sender)
            .Include(n => n.Receiver)
            .Where(f => f.ReceiverId == userId && f.Status == FriendShipStatus.Pending)
            .ToListAsync();

        return friendRequestsSent;
    }

    public async Task<List<Friendship>> GetFriendsAsync(int userId)
    {
        var friends = await _context.Friendships
            .Include(n => n.Sender)
            .Include(n => n.Receiver)
            .Where(n => n.SenderId == userId || n.ReceiverId == userId)
            .ToListAsync();

        return friends;
    }
}