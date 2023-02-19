using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class UrlService : IUrlService
    {
        private readonly UrlContext _urlContext;

        public UrlService(UrlContext urlContext)
        {
            _urlContext = urlContext;
        }

        public IEnumerable<UrlInfo> GetLinks()
        {
            return _urlContext.Urls;
        }
        public string GetOriginalUrl(string shortenedUrl)
        {
            string originalUrl = "";

           string shorterUrl = shortenedUrl.Substring(shortenedUrl.LastIndexOf('/') + 1);
           var urlInfo = _urlContext.Urls.FirstOrDefault(u => u.ShortenedUrl == shorterUrl);
            if (urlInfo != null)
            {
                originalUrl = urlInfo.Url;   
            }
            else
            {
                originalUrl = "This Shortened URL doesn't exist yet";
            }
           return originalUrl;
        }

        public string GetShortenedUrl(string url,string userEmail)
        {
            //var users = _userContext.Users;
            var urlInfo = _urlContext.Urls.FirstOrDefault(u => u.Url.Equals(url));
            if (urlInfo == null)
            {
                urlInfo = new UrlInfo { Url = url, ShortenedUrl = StringToChars()};
                while (_urlContext.Urls.FirstOrDefault(u => u.ShortenedUrl.Equals(urlInfo.ShortenedUrl)) != null)
                {
                    urlInfo.ShortenedUrl = StringToChars();
                }              
                var user = _urlContext.Users.FirstOrDefault(u => u.Email == userEmail);
                urlInfo.UserId = user.Id;
                _urlContext.Urls.Add(urlInfo);
                if(user.Links == null)
                {
                    user.Links = new List<UrlInfo>();
                }
                 user.Links.Append(urlInfo);
                _urlContext.SaveChanges();
            }
            return urlInfo.ShortenedUrl;
        }
        public string ApiGetShortenedUrl(string url)
        {
            _urlContext.Database.EnsureCreated();
            var urlInfo = _urlContext.Urls.FirstOrDefault(u => u.Url.Equals(url));
            if (urlInfo == null)
            {
                urlInfo = new UrlInfo { Url = url, ShortenedUrl = StringToChars() };
                while (_urlContext.Urls.FirstOrDefault(u => u.ShortenedUrl.Equals(urlInfo.ShortenedUrl)) != null)
                {
                    urlInfo.ShortenedUrl = StringToChars();
                }
                _urlContext.Urls.Add(urlInfo);
                _urlContext.SaveChanges();
            }
            return urlInfo.ShortenedUrl;
        }
        public string Redirect(string ShortenedUrl)
        {
            var originalUrlInfo = _urlContext.Urls.FirstOrDefault(u => u.ShortenedUrl.Equals(ShortenedUrl));
            if (originalUrlInfo == null)
            {
                return "";
            }
            originalUrlInfo.Clicks++;
            _urlContext.SaveChanges();
            return originalUrlInfo.Url;
        }
        private string StringToChars()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}
