using JetBrains.Annotations;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Volo.Abp;
using Volo.Abp.Security.Claims;

namespace TMT.Security.Principal
{
    public static class AbpClaimsIdentityExtensions
    {
        public static string FindUserId([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == AbpClaimTypes.UserId);
            if (userIdOrNull == null || String.IsNullOrEmpty(userIdOrNull.Value))
            {
                return null;
            }
            return userIdOrNull.Value;
        }

        public static string FindUserId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var userIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == AbpClaimTypes.UserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return userIdOrNull.Value;
        }

        public static Guid? FindTenantId([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var tenantIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == AbpClaimTypes.TenantId);
            if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(tenantIdOrNull.Value);
        }

        public static Guid? FindTenantId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var tenantIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == AbpClaimTypes.TenantId);
            if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(tenantIdOrNull.Value);
        }

        public static string FindClientId([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var clientIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == AbpClaimTypes.ClientId);
            if (clientIdOrNull == null || clientIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return clientIdOrNull.Value;
        }

        public static string FindClientId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var clientIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == AbpClaimTypes.ClientId);
            if (clientIdOrNull == null || clientIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return clientIdOrNull.Value;
        }

        public static Guid? FindEditionId([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var editionIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == AbpClaimTypes.EditionId);
            if (editionIdOrNull == null || editionIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(editionIdOrNull.Value);
        }

        public static Guid? FindEditionId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var editionIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == AbpClaimTypes.EditionId);
            if (editionIdOrNull == null || editionIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(editionIdOrNull.Value);
        }
    }
}