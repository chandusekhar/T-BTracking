using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.Departments
{
    public class DepartmentManager : DomainService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentManager(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        public Department CreateAsync(
         [NotNull] string name,
          [NotNull]string idManager
          )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

          
            return new Department(
                GuidGenerator.Create(),
                name,
                idManager
            );
        }
    }
}
