using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UrlInfo
    {
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }
        [Required,MaxLength(6),MinLength(6)]
        public string ShortenedUrl { get; set; }
        public int Clicks { get; set; } = 0;
        public int? UserId { get; set; }
        public virtual ICollection<ClickInfo>? ClicksInfo { get; set; }
    }
}
