using System;
using System.Security.Claims;

namespace TMT.Security.Claims
{
    public interface ITMTCurrentPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }

        IDisposable Change(ClaimsPrincipal principal);
    }
}