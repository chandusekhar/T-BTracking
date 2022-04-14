using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp;

namespace BugTracking.Departments
{
    public class Department : TMTAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string IdManager { get; set; }

        private Department()
        {

        }
        internal Department(
            Guid id,
            [NotNull] string name,
            [NotNull] string idmanager ): base(id)
        {
            SetName(name);
            IdManager = idmanager;
           
        }
        internal Department ChangeName([NotNull] string departmentName)
        {
            SetName(departmentName);
            return this;
        }

        private void SetName([NotNull] string departmentName)
        {
            Name = Check.NotNullOrWhiteSpace(
                departmentName,
                nameof(departmentName)
            );
        }
    }
}
