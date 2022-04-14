using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.Assignees
{
    public class AssigneeManager : DomainService
    {
        private readonly IAssigneeRepository _assigneeRepository;
        public AssigneeManager(IAssigneeRepository assigneeRepository)
        {
            _assigneeRepository = assigneeRepository;
            //push change
        }

        public Assignee CreateAsync(
            Guid issueID,
            string userID
            )
        {
            return new Assignee(
                GuidGenerator.Create(),
                issueID,
                userID

            );
        }
    }
}
