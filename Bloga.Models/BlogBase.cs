using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bloga.Models
{
    public abstract class BlogBase
    {
        public int ApplicationUserId { get; set; }
        [Required]
        [MinLength(300, ErrorMessage = "Must be 300-5000 characters")]
        [MaxLength(5000, ErrorMessage = "Must be 300-5000 characters")]
        public string Content { get; set; }

        public int? PhotoId { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Must be 10-50 characters")]
        [MaxLength(50, ErrorMessage = "Must be 10-50 characters")]
        public string Title { get; set; }
    }

    public class BlogUpsert : BlogBase
    {
        public int BlogId { get; set; }
    }

    public class BlogPaging
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;
    }

    public class PagedResults<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalCount { get; set; }
    }
}