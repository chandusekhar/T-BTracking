using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BugTracking.IShareDto
{
    public interface IRepositoryDeleteGetByIdShare<T1, T2>
    {
        Task<T1> GetByIdAsync(T2 Id);

        Task DeleteByIdAsync(T2 Id);
    }
}
