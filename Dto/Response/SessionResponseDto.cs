using NeatPath.Models;

namespace NeatPath.Dto.Response
{
    public class SessionResponseDto
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserShortResponseDto User { get; set; }
    }
}
