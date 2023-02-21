using System.ComponentModel.DataAnnotations;
using System.Net;

namespace WebApplication1.Models
{
    public class ClickInfo
    {
        [Key]
        public DateTime ClickDateTimeUtc { get; set; }
        public IPAddress? IpAddress { get; set; }
    }
}
