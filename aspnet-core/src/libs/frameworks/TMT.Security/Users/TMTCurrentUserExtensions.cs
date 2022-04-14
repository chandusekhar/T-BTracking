using JetBrains.Annotations;
using System;
using System.Diagnostics;

namespace TMT.Security.Users
{
    public static class TMTCurrentUserExtensions
    {
        [CanBeNull]
        public static string FindClaimValue(this ITMTCurrentUser currentUser, string claimType)
        {
            return currentUser.FindClaim(claimType)?.Value;
        }

        public static T FindClaimValue<T>(this ITMTCurrentUser currentUser, string claimType)
            where T : struct
        {
            var value = currentUser.FindClaimValue(claimType);
            if (value == null)
            {
                return default;
            }

            return value.To<T>();
        }

        public static string GetId(this ITMTCurrentUser currentUser)
        {
            Debug.Assert(currentUser.Id != null, "currentUser.Id != null");

            return currentUser.Id;
        }
    }
}