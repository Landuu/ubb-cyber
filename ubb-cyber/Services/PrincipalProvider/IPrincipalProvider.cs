using System.Security.Claims;

namespace ubb_cyber.Services.PrincipalProvider
{
    public interface IPrincipalProvider
    {
        ClaimsPrincipal? User { get; }
    }
}