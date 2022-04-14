using AutoMapper;
using BugTracking.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BugTracking.Departments
{
    public class EfCoreDepartmentRepository : EfCoreRepository<BugTrackingDbContext, Department, Guid>, IDepartmentRepository
    {
        public EfCoreDepartmentRepository(
             IDbContextProvider<BugTrackingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {}

        public async Task<List<Department>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .OrderBy(a => sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }
    }
}
