using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.Projects
{
    public class ProjectManager : DomainService
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectManager(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<Project> CreateAsync(
           [NotNull] string name,
             string nzColor,
             Guid projectIdTfs,
             string witType
           )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var existingProject = await _projectRepository.FindByNameAsync(name);
            if (existingProject != null)
            {
                throw new ProjectNameAlreadyExistsException(name);
            }
            return new Project(
                GuidGenerator.Create(),
                name,
                nzColor,
                projectIdTfs,
                witType
            );
        }
        public async Task ChangeNameAsync(
          [NotNull] Project project,
          [NotNull] string newName)
        {
            Check.NotNull(project, nameof(project));
            Check.NotNullOrWhiteSpace(newName, nameof(newName));

            var existingProject = await _projectRepository.FindByNameAsync(newName);
            if (existingProject != null && existingProject.Id != project.Id)
            {
                throw new ProjectNameAlreadyExistsException(newName);
            }

            project.ChangeName(newName);
        }
    }
}
