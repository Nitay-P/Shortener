using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UrlInfo
    {
        [Required,Key]
        public string Url { get; set; }
        [Required,MaxLength(6),MinLength(6)]
        public string ShortenedUrl { get; set; }
        public int Clicks { get; set; } = 0;
        public int? UserId { get; set; }
    }
}
