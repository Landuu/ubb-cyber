using System.Security.Claims;

namespace ubb_cyber.Utils
{
    public static class UserExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            if (principal == null || principal.Identity == null) return false;
            var roles = principal.FindAll(x => x.Type == ClaimTypes.Role);
            if (!roles.Any()) return false;
            return roles.Any(x => x.Value == "admin");       
        }
    }
}
