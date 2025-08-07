﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.Data.Models
{
    public class Hashtag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }      
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
