using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace TMT.Security.Claims
{
    public static class TMTCurrentPrincipalAccessorExtensions
    {
        public static IDisposable Change(this ITMTCurrentPrincipalAccessor currentPrincipalAccessor, Claim claim)
        {
            return currentPrincipalAccessor.Change(new[] { claim });
        }

        public static IDisposable Change(this ITMTCurrentPrincipalAccessor currentPrincipalAccessor, IEnumerable<Claim> claims)
        {
            return currentPrincipalAccessor.Change(new ClaimsIdentity(claims));
        }

        public static IDisposable Change(this ITMTCurrentPrincipalAccessor currentPrincipalAccessor, ClaimsIdentity claimsIdentity)
        {
            return currentPrincipalAccessor.Change(new ClaimsPrincipal(claimsIdentity));
        }
    }
}