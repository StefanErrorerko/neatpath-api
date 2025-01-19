using System.ComponentModel.DataAnnotations;

namespace NeatPath.Dto.Request
{
    public class UserChangePasswordDto
    {
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string CurrentPassword { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}
