using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.Azures
{
    public class AzureManager : DomainService
    {
        private readonly IAzureRepository _azureRepository;

        public AzureManager(IAzureRepository azureRepository)
        {
            _azureRepository = azureRepository;
        }

        public Azure CreateAsync(
            [NotNull] string host,
            [NotNull] string collection
            )
        {
            Check.NotNullOrWhiteSpace(host, nameof(host));
            Check.NotNullOrWhiteSpace(collection, nameof(collection));
            if (_azureRepository.Any())
            {
                throw new UserFriendlyException("Exist Host");
            }
            return new Azure(
                GuidGenerator.Create(),
                host,
                collection
            );
        }
    }
}
