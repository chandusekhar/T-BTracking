using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BugTracking.IShareDto
{
    public interface IRepositoryCreateUpdateShareDto<T1, T2>
    {
        //Task UpdateAsync(T1 Entity, T2 Id);
        Task InsertAsync(T1 Entity);
    }
}
