using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data.Helpers;
using SocialMediaApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.Data.Services
{
    public class HashtagsService : IHashtagsService
    {
        private readonly AppDbContext _context;
        public HashtagsService(AppDbContext context)
        {
            _context = context;
        }
        public async Task ProcessHashtagsForNewPostAsync(string content)
        {
            var postHashtags = HashtagHelper.GetHashtags(content);
            foreach (var hashtag in postHashtags)
            {
                var hashtagDb = await _context.Hashtags
                    .FirstOrDefaultAsync(h => h.Name == hashtag);
                if (hashtagDb != null)
                {
                    hashtagDb.Count++;
                    hashtagDb.DateUpdated = DateTime.UtcNow;

                    _context.Hashtags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newHashtag = new Hashtag
                    {
                        Name = hashtag,
                        Count = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };
                    await _context.Hashtags.AddAsync(newHashtag);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task ProcessHashtagsForRemovePostAsync(string content)
        {
            var posthashtags = HashtagHelper.GetHashtags(content);
            foreach (var hashtag in posthashtags)
            {
                var hashtagdb = await _context.Hashtags
                    .FirstOrDefaultAsync(h => h.Name == hashtag);
                if (hashtagdb != null)
                {
                    hashtagdb.Count--;
                    hashtagdb.DateUpdated = DateTime.UtcNow;

                    _context.Hashtags.Update(hashtagdb);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
