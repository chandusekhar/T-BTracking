using BugTracking.Issues;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AuditLogging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace BugTracking.Follows
{
    class AuditLogHandler
        : ILocalEventHandler<AuditLogEventDto>,
          ITransientDependency
    {
        private readonly IIssueRepository _issueRepository;
        public AuditLogHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }
        public async Task HandleEventAsync(AuditLogEventDto eventData)
        {
            var issue = _issueRepository.IgnoreQueryFilters().Where(i => i.Id.ToString() == eventData.IssueId).FirstOrDefault();
            issue.SetProperty("AuditLogId", eventData.AuditLogId);
            await _issueRepository.UpdateAsync(issue);
        }
    }
}
