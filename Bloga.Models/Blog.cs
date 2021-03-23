using System;

namespace Bloga.Models
{
    public class Blog : BlogBase
    {
        public int BlogId { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime PublishDate { get; set; }
        public string Username { get; set; }
    }
}