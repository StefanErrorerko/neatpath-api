using System.ComponentModel.DataAnnotations;

namespace NeatPath.Dto.Request
{
    public class SessionTokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}
