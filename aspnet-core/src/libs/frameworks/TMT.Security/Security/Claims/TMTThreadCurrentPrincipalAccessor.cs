using System.Security.Claims;
using System.Threading;
using Volo.Abp.DependencyInjection;

namespace TMT.Security.Claims
{
    public class TMTThreadCurrentPrincipalAccessor : TMTCurrentPrincipalAccessorBase, ISingletonDependency
    {
        protected override ClaimsPrincipal GetClaimsPrincipal()
        {
            return Thread.CurrentPrincipal as ClaimsPrincipal;
        }
    }
}