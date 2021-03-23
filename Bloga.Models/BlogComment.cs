using System;

namespace Bloga.Models
{
    public class BlogComment : BlogCommentBase
    {
        public int BlogCommentId { get; set; }
        public string Username { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}