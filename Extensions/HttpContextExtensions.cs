using WebApplication1.Models;

namespace WebApplication1.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetFullPath(this HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}/s/";
        }
        public static string GetHref(this UrlInfo urlInfo,HttpContext httpContext)
        {
            return $"{httpContext.GetFullPath()}{urlInfo.ShortenedUrl}";
        }
        public static string GetCurrentFullPath(this HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}";
        }
    }
}
