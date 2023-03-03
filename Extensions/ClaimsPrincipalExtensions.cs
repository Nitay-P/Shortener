using System.Security.Claims;

namespace WebApplication1.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Email)!;
        }
        public static bool IsThirdParty(this ClaimsPrincipal claimsPrincipal)
        {
            return bool.Parse(claimsPrincipal.FindFirstValue("ThirdParty")!);
        }
    }
}
