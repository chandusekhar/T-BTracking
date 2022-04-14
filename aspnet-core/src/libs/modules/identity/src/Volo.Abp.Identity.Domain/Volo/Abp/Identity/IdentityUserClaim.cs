using JetBrains.Annotations;
using System;
using System.Security.Claims;

namespace Volo.Abp.Identity
{
    /// <summary>
    /// Represents a claim that a user possesses.
    /// </summary>
    public class IdentityUserClaim : IdentityClaim
    {
        /// <summary>
        /// Gets or sets the primary key of the user associated with this claim.
        /// </summary>
        public virtual string UserId { get; protected set; }

        protected IdentityUserClaim()
        {
        }

        protected internal IdentityUserClaim(Guid id, string userId, [NotNull] Claim claim, Guid? tenantId)
            : base(id, claim, tenantId)
        {
            UserId = userId;
        }

        public IdentityUserClaim(Guid id, string userId, [NotNull] string claimType, string claimValue, Guid? tenantId)
            : base(id, claimType, claimValue, tenantId)
        {
            UserId = userId;
        }
    }
}