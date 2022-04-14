using JetBrains.Annotations;
using System;

namespace Volo.Abp.Users
{
    public interface IUserData
    {
        string Id { get; }

        Guid? TenantId { get; }

        string UserName { get; }

        string Name { get; }

        string Surname { get; }

        [CanBeNull]
        string Email { get; }

        bool EmailConfirmed { get; }

        [CanBeNull]
        string PhoneNumber { get; }

        bool PhoneNumberConfirmed { get; }
    }
}