using NeatPath.Models.Interfaces;

namespace NeatPath.Models
{
    public class Url : ITrackable
    {
        public int Id { get; set; }
        public string OriginalUrl {  get; set; }
        public string ShortUrl { get; set; }
        public string Hash { get; set; }
        public int ClickCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User User { get; set; }
    }
}
