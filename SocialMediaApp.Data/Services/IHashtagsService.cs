using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.Data.Services
{
    public interface IHashtagsService
    {
        Task ProcessHashtagsForNewPostAsync(string content);
        Task ProcessHashtagsForRemovePostAsync(string content);
    }
}
