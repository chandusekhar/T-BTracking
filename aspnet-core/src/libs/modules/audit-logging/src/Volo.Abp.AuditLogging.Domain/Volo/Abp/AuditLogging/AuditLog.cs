using BugTracking.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.AuditLogging
{
    [DisableAuditing]
    public class AuditLog : AggregateRoot<Guid>, IMultiTenant
    {
        public string ApplicationName { get; set; }

        public string UserId { get; protected set; }

        public string UserName { get; protected set; }

        public Guid? TenantId { get; protected set; }

        public string TenantName { get; protected set; }

        public Guid? ImpersonatorUserId { get; protected set; }

        public Guid? ImpersonatorTenantId { get; protected set; }

        public DateTime ExecutionTime { get; protected set; }

        public int ExecutionDuration { get; protected set; }

        public string ClientIpAddress { get; protected set; }

        public string ClientName { get; protected set; }

        public string ClientId { get; set; }

        public string CorrelationId { get; set; }

        public string BrowserInfo { get; protected set; }

        public string HttpMethod { get; protected set; }

        public string Url { get; protected set; }

        public string Exceptions { get; protected set; }

        public string Comments { get; protected set; }

        public int? HttpStatusCode { get; set; }

        public ICollection<EntityChange> EntityChanges { get; protected set; }

        public ICollection<AuditLogAction> Actions { get; protected set; }

        protected AuditLog()
        {
        }

        public AuditLog(IGuidGenerator guidGenerator, AuditLogInfo auditInfo, AppUser user)
            : base(guidGenerator.Create())
        {
            ApplicationName = auditInfo.ApplicationName.Truncate(AuditLogConsts.MaxApplicationNameLength);
            TenantId = auditInfo.TenantId;
            TenantName = auditInfo.TenantName.Truncate(AuditLogConsts.MaxTenantNameLength);
            UserId = user.Id;
            UserName = user.Name;
            ExecutionTime = auditInfo.ExecutionTime;
            ExecutionDuration = auditInfo.ExecutionDuration;
            ClientIpAddress = auditInfo.ClientIpAddress.Truncate(AuditLogConsts.MaxClientIpAddressLength);
            ClientName = auditInfo.ClientName.Truncate(AuditLogConsts.MaxClientNameLength);
            ClientId = auditInfo.ClientId.Truncate(AuditLogConsts.MaxClientIdLength);
            CorrelationId = auditInfo.CorrelationId.Truncate(AuditLogConsts.MaxCorrelationIdLength);
            BrowserInfo = auditInfo.BrowserInfo.Truncate(AuditLogConsts.MaxBrowserInfoLength);
            HttpMethod = auditInfo.HttpMethod.Truncate(AuditLogConsts.MaxHttpMethodLength);
            Url = auditInfo.Url.Truncate(AuditLogConsts.MaxUrlLength);
            HttpStatusCode = auditInfo.HttpStatusCode;
            ImpersonatorUserId = auditInfo.ImpersonatorUserId;
            ImpersonatorTenantId = auditInfo.ImpersonatorTenantId;

            ExtraProperties = new ExtraPropertyDictionary();
            if (auditInfo.ExtraProperties != null)
            {
                foreach (var pair in auditInfo.ExtraProperties)
                {
                    ExtraProperties.Add(pair.Key, pair.Value);
                }
            }

            EntityChanges = auditInfo
                                .EntityChanges?
                                .Select(entityChangeInfo => new EntityChange(guidGenerator, Id, entityChangeInfo, tenantId: auditInfo.TenantId))
                                .ToList()
                            ?? new List<EntityChange>();

            Actions = auditInfo
                          .Actions?
                          .Select(auditLogActionInfo => new AuditLogAction(
                                guidGenerator.Create(), Id, auditLogActionInfo,
                                tenantId: auditInfo.TenantId,
                                auditLogActionType: auditLogActionInfo.GetProperty("AuditLogActionType"),
                                entityId: auditLogActionInfo.GetProperty("EntityId")
                            ))
                          .ToList()
                      ?? new List<AuditLogAction>();

            Exceptions = auditInfo
                .Exceptions?
                .JoinAsString(Environment.NewLine)
                .Truncate(AuditLogConsts.MaxExceptionsLengthValue);

            Comments = auditInfo
                .Comments?
                .JoinAsString(Environment.NewLine)
                .Truncate(AuditLogConsts.MaxCommentsLength);
        }
    }
}