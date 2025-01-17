﻿using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class UrlService : IUrlService
    {
        private readonly UrlContext _urlContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlService(UrlContext urlContext, IHttpContextAccessor httpContextAccessor)
        {
            _urlContext = urlContext;
            _httpContextAccessor = httpContextAccessor;

        }

        public IEnumerable<ClickInfo> GetClickInfo()
        {
            return _urlContext.ClicksInfo;
        }
        public IEnumerable<UrlInfo> GetLinks()
        {
            _urlContext.Urls.Include(u => u.ClicksInfo);
            return _urlContext.Urls;
        }
        public string GetOriginalUrl(string shortenedUrl)
        {
            string originalUrl = "";

           string shorterUrl = shortenedUrl[(shortenedUrl.LastIndexOf('/') + 1)..];
           var urlInfo = _urlContext.Urls.FirstOrDefault(u => u.ShortenedUrl == shorterUrl);
            if (urlInfo != null)
            {
                originalUrl = urlInfo.Url!;   
            }
            else
            {
                originalUrl = "This Shortened URL doesn't exist yet";
            }
           return originalUrl;
        }

        public string GetShortenedUrl(string url,string userEmail)
        {
            var user = _urlContext.Users.FirstOrDefault(u => u.Email == userEmail);
            var urlInfo = _urlContext.Urls.FirstOrDefault(u => u.Url.Equals(url) && u.UserId == null);
            if (user != null)
            {
                urlInfo = _urlContext.Urls.FirstOrDefault(u => u.Url.Equals(url) && u.UserId == user.Id);
            }
            if (urlInfo == null)
            {
                urlInfo = new UrlInfo { Url = url, ShortenedUrl = StringToChars() };
                while (_urlContext.Urls.FirstOrDefault(u => u.ShortenedUrl.Equals(urlInfo.ShortenedUrl)) != null)
                {
                    urlInfo.ShortenedUrl = StringToChars();
                }
                if(user != null)
                {
                    urlInfo.UserId = user.Id;
                    user.Links ??= new List<UrlInfo>();
                    user.Links.Add(urlInfo);             
                }
                _urlContext.Urls.Add(urlInfo);
            }
           
            _urlContext.SaveChanges();
            return urlInfo.ShortenedUrl!;
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
            return urlInfo.ShortenedUrl!;
        }
        public async Task <string> Redirect(string shortenedUrl)
        {
            var originalUrlInfo = _urlContext.Urls.FirstOrDefault(u => u.ShortenedUrl.Equals(shortenedUrl));
            if (originalUrlInfo == null)
            {
                return "";
            }
            originalUrlInfo.ClicksInfo ??= new List<ClickInfo>();
            originalUrlInfo.ClicksInfo.Add(new ClickInfo { ClickDateTimeUtc = DateTime.UtcNow,IpAddress = _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress});
            originalUrlInfo.Clicks++;
            await _urlContext.SaveChangesAsync();
            return originalUrlInfo.Url!;
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
