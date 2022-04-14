using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.Statuss
{
    public class StatusManager : DomainService
    {
        private readonly IStatusRepository _statusRepository;

        public StatusManager(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        public async Task<Status> CreateAsync(
            [NotNull] string name,
            int currentIndex,
            bool isDefault,
            string nzColor)

        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var existingAuthor = await _statusRepository.FindByNameAsync(name);
            if (existingAuthor != null)
            {
                throw new StatusAlreadyExistsException(name);
            }

            return new Status(
                GuidGenerator.Create(),
                name,
                currentIndex,
                isDefault,
                nzColor
                );
        }

        public async Task ChangeNameAsync(
            [NotNull] Status status,
            [NotNull] string newName)
        {
            Check.NotNull(status, nameof(status));
            Check.NotNullOrWhiteSpace(newName, nameof(newName));

            var existingAuthor = await _statusRepository.FindByNameAsync(newName);
            if (existingAuthor != null && existingAuthor.Id != status.Id)
            {
                throw new StatusAlreadyExistsException(newName);
            }

            status.ChangeName(newName);
        }

    }
}
