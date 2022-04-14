using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.Categories
{
    public class CategoryManager: DomainService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> CreateAsync(
            [NotNull] string name
            )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var existingCategory = await _categoryRepository.FindByCategoryNameAsync(name);
            if (existingCategory != null)
            {
                throw new CategoryNameAlreadyExistsException(name);
            }
            return new Category(
                GuidGenerator.Create(),
                name
            );
        }
    }
}
