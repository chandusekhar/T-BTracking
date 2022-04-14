﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Volo.Abp.Identity
{
    public class IdentityDataSeeder : ITransientDependency, IIdentityDataSeeder
    {
        protected IGuidGenerator GuidGenerator { get; }
        protected IIdentityRoleRepository RoleRepository { get; }
        protected IIdentityUserRepository UserRepository { get; }
        protected ILookupNormalizer LookupNormalizer { get; }
        protected IdentityUserManager UserManager { get; }
        protected IdentityRoleManager RoleManager { get; }
        protected ICurrentTenant CurrentTenant { get; }
        protected IOptions<IdentityOptions> IdentityOptions { get; }

        public IdentityDataSeeder(
            IGuidGenerator guidGenerator,
            IIdentityRoleRepository roleRepository,
            IIdentityUserRepository userRepository,
            ILookupNormalizer lookupNormalizer,
            IdentityUserManager userManager,
            IdentityRoleManager roleManager,
            ICurrentTenant currentTenant,
            IOptions<IdentityOptions> identityOptions)
        {
            GuidGenerator = guidGenerator;
            RoleRepository = roleRepository;
            UserRepository = userRepository;
            LookupNormalizer = lookupNormalizer;
            UserManager = userManager;
            RoleManager = roleManager;
            CurrentTenant = currentTenant;
            IdentityOptions = identityOptions;
        }

        [UnitOfWork]
        public virtual async Task<IdentityDataSeedResult> SeedAsync(
            string adminEmail,
            string adminPassword,
            Guid? tenantId = null)
        {
            Check.NotNullOrWhiteSpace(adminEmail, nameof(adminEmail));
            Check.NotNullOrWhiteSpace(adminPassword, nameof(adminPassword));

            using (CurrentTenant.Change(tenantId))
            {
                await IdentityOptions.SetAsync();

                var result = new IdentityDataSeedResult();
                //"admin" user
                const string adminUserName = "admin";
                var adminUser = await UserRepository.FindByNormalizedUserNameAsync(
                    LookupNormalizer.NormalizeName(adminUserName)
                );

                if (adminUser != null)
                {
                    return result;
                }

                adminUser = new IdentityUser(
                    (long)(DateTime.UtcNow - new DateTime(2021, 01, 01)).TotalMilliseconds + (new Random().Next(99999)).ToString(),
                    adminUserName,
                    adminEmail,
                    tenantId
                )
                {
                    Name = adminUserName
                };

                (await UserManager.CreateAsync(adminUser, adminPassword, validatePassword: false)).CheckErrors();
                result.CreatedAdminUser = true;

                //"admin" role
                const string adminRoleName = "admin";
                var adminRole =
                    await RoleRepository.FindByNormalizedNameAsync(LookupNormalizer.NormalizeName(adminRoleName));
                if (adminRole == null)
                {
                    adminRole = new IdentityRole(
                        GuidGenerator.Create(),
                        adminRoleName,
                        tenantId
                    )
                    {
                        IsStatic = true,
                        IsPublic = true
                    };

                    var userRole = new IdentityRole(
                        GuidGenerator.Create(),
                        "user",
                        tenantId
                    )
                    {
                        IsStatic = true,
                        IsPublic = true
                    };

                    (await RoleManager.CreateAsync(adminRole)).CheckErrors();
                    (await RoleManager.CreateAsync(userRole)).CheckErrors();
                    result.CreatedAdminRole = true;
                }

                (await UserManager.AddToRoleAsync(adminUser, adminRoleName)).CheckErrors();

                return result;
            }
        }
    }
}