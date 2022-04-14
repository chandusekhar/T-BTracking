using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Azures
{
    public class AzureDto : EntityDto<Guid>
    {
        public string Host { get; set; }
        public string Collection { get; set; }
    }
}
