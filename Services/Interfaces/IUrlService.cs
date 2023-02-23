using System.Security.AccessControl;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IUrlService
    {
        public IEnumerable<UrlInfo> GetLinks();
        public string GetShortenedUrl(string url, string userEmail);
        public IEnumerable<ClickInfo> GetClickInfo();
        public string ApiGetShortenedUrl(string url);
        public string GetOriginalUrl(string shortenedUrl);
        public Task<string> Redirect(string url);
    }
}
