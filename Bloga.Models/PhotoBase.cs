namespace Bloga.Models
{
    public class PhotoBase
    {
        public int ApplicationUserId { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public string PublicId { get; set; }
    }
}