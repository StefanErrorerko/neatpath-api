using NeatPath.Models;
using System.ComponentModel.DataAnnotations;

namespace NeatPath.Dto.Request
{
    public class UrlCreateDto
    {
        [Required]
        public string OriginalUrl { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
