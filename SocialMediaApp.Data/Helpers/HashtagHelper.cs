﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SocialMediaApp.Data.Helpers
{
    public static class HashtagHelper
    {
        public static List<string> GetHashtags(string postText)
        {
            var hashtagPattern = new Regex(@"#\w+");
            var matches = hashtagPattern.Matches(postText)
                .Select(m => m.Value.TrimEnd('.',',','!','?').ToLower())
                .Distinct()
                .ToList();

            return matches;
        }
    }
}
