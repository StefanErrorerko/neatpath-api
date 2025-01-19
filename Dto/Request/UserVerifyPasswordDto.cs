using System.ComponentModel.DataAnnotations;

namespace NeatPath.Dto.Request
{
    public class UserVerifyPasswordDto
    {
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
