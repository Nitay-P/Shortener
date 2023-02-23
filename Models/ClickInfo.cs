using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace WebApplication1.Models
{
    public class ClickInfo
    {
        public int Id { get; set; }
        public DateTime ClickDateTimeUtc { get; set; }
        public IPAddress? IpAddress { get; set; }
        [ForeignKey(nameof(UrlInfo))]
        public int UrlInfoId { get; set; }
    }
}
