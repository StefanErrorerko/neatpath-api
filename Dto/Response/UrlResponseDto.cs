using NeatPath.Models;

namespace NeatPath.Dto.Response
{
    public class UrlResponseDto
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortUrl { get; set; }
        public string Hash { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserShortResponseDto User { get; set; }
    }
}
