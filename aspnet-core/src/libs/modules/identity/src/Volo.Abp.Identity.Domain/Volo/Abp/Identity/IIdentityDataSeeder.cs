using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

namespace Volo.Abp.Identity
{
    public interface IIdentityDataSeeder
    {
        Task<IdentityDataSeedResult> SeedAsync(
            [NotNull] string adminEmail,
            [NotNull] string adminPassword,
            Guid? tenantId = null);
    }
}