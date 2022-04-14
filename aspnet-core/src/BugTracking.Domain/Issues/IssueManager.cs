using BugTracking.LevelEnum;
using BugTracking.PriorityEnum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.Issues
{ 
    public class IssueManager : DomainService
    {
        private readonly IIssueRepository _issueRepository;
        public IssueManager(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }
        public async Task<Issue> CreateAsync(
            [NotNull] string name,
            string description,
            Priority priority,//
            Level isselevel,
            Guid? catId,
            Guid projectId,
            Guid stastusId,
            DateTime? dueDate,
            DateTime? startDate,
            DateTime? finishDate,
            int currentIndex,
            int idWIWT,
            int rev,
            Guid idParent
            )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var existingIssue = await _issueRepository.FindByIssueNameAsync(name, projectId);
            if (existingIssue != null)
            {
                throw new IssueNameAlreadyExistsException(name);
            }
            return new Issue(
                GuidGenerator.Create(),
                name,
                description,
                priority,
                isselevel,
                catId,
                projectId,
                stastusId,
                dueDate,
                startDate,
                finishDate,
                currentIndex,
                idWIWT,
                rev,
                idParent
            );
        }
    }
}
