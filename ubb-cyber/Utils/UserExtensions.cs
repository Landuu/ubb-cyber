using System.Security.Claims;

namespace ubb_cyber.Utils
{
    public static class UserExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated) return false;
            var roles = principal.FindAll(x => x.Type == ClaimTypes.Role);
            if (!roles.Any()) return false;
            return roles.Any(x => x.Value == "admin");       
        }

        public static int? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated) return null;
            var id = principal.FindFirstValue(ClaimTypes.Sid);
            return id != null ? Convert.ToInt32(id) : null;
        }
    }
}
