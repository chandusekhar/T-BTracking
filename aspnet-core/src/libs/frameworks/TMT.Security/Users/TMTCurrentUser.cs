using System;
using System.Linq;
using System.Security.Claims;
using TMT.Security.Principal;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace TMT.Security.Users
{
    public class TMTCurrentUser : ITMTCurrentUser, ITransientDependency
    {
        private static readonly Claim[] EmptyClaimsArray = new Claim[0];

        public virtual bool IsAuthenticated => Id != null;

        public virtual string Id => _principalAccessor.Principal?.FindUserId();

        public virtual string UserName => this.FindClaimValue(AbpClaimTypes.UserName);

        public virtual string Name => this.FindClaimValue(AbpClaimTypes.Name);

        public virtual string SurName => this.FindClaimValue(AbpClaimTypes.SurName);

        public virtual string PhoneNumber => this.FindClaimValue(AbpClaimTypes.PhoneNumber);

        public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);

        public virtual string Email => this.FindClaimValue(AbpClaimTypes.Email);

        public virtual bool EmailVerified => string.Equals(this.FindClaimValue(AbpClaimTypes.EmailVerified), "true", StringComparison.InvariantCultureIgnoreCase);

        public virtual Guid? TenantId => _principalAccessor.Principal?.FindTenantId();

        public virtual string[] Roles => FindClaims(AbpClaimTypes.Role).Select(c => c.Value).ToArray();

        private readonly ICurrentPrincipalAccessor _principalAccessor;

        public TMTCurrentUser(ICurrentPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }

        public virtual Claim FindClaim(string claimType)
        {
            return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
        }

        public virtual Claim[] FindClaims(string claimType)
        {
            return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
        }

        public virtual Claim[] GetAllClaims()
        {
            return _principalAccessor.Principal?.Claims.ToArray() ?? EmptyClaimsArray;
        }

        public virtual bool IsInRole(string roleName)
        {
            return FindClaims(AbpClaimTypes.Role).Any(c => c.Value == roleName);
        }
    }
}