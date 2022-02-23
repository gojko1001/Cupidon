using System.Security.Claims;

namespace DatingApp.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static int GetId(this ClaimsPrincipal user) => int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        public static string GetUsername(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.Name)?.Value;
    }
}
