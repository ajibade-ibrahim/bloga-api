using System.ComponentModel.DataAnnotations;

namespace Bloga.Models
{
    public abstract class BlogCommentBase
    {
        public int ApplicationUserId { get; set; }
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Must be 10-300 characters")]
        [MaxLength(300, ErrorMessage = "Must be 10-300 characters")]
        public string Content { get; set; }
        public int? ParentBlogCommentId { get; set; }
    }
}