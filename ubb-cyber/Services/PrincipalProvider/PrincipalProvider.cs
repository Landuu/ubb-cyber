using System.Security.Claims;

namespace ubb_cyber.Services.PrincipalProvider
{
    public class PrincipalProvider : IPrincipalProvider
    {
        public ClaimsPrincipal? User { get; private set; }

        public PrincipalProvider(IHttpContextAccessor accessor)
        {
            User = accessor.HttpContext?.User;
        }
    }
}
