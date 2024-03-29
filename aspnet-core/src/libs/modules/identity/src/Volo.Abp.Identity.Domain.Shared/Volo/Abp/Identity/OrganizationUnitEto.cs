﻿using System;

namespace Volo.Abp.Identity
{
    [Serializable]
    public class OrganizationUnitEto
    {
        public string Id { get; set; }

        public Guid? TenantId { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }
    }
}