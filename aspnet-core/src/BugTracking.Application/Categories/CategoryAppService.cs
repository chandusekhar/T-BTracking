using BugTracking.Assignees;
using BugTracking.Hub;
using BugTracking.Issues;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Categories
{
    [Authorize("BugTracking.Users")]
    //[RemoteService(IsEnabled = false)]
    public class CategoryAppService : BugTrackingAppService, ICategoryAppService
    {
        private IHubContext<SignalR> _hub;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly CategoryManager _categoryManager;
        private readonly IStatusRepository _statusRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly IRepository<Issue, Guid> _isssueRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly UserManager _userManager;
        public CategoryAppService(
            ICategoryRepository categoryRepository,
            IIssueRepository issueRepository,
            IProjectRepository projectRepository,
            CategoryManager categoryManager,
            IStatusRepository statusRepository,
            IRepository<Issue, Guid> isssueRepsitory,
            IRepository<AppUser, string> userRepository,
            IAssigneeRepository assigneeRepository,
            ITMTCurrentUser tMTCurrentUser,
            UserManager userManager,
            IHubContext<SignalR> hub)

        {
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _categoryManager = categoryManager;
            _issueRepository = issueRepository;
            _statusRepository = statusRepository;
            _projectRepository = projectRepository;
            _hub = hub;
            _statusRepository = statusRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _isssueRepository = isssueRepsitory;
            _assigneeRepository = assigneeRepository;
            _tmtCurrentUser = tMTCurrentUser;
            _userManager = userManager;
        }
        public async Task<ListResultDto<CategoryDto>> GetListAsync()
        {
            var categories = await _categoryRepository.GetListAsync();
            List<CategoryDto> listCategoryDto = new List<CategoryDto>();

            foreach(Category category in categories){

                var categoryMap = ObjectMapper.Map<Category, CategoryDto>(category);
                categoryMap.count = _isssueRepository.Count(x => x.CategoryID == category.Id);
                categoryMap.CreatedBy = category.CreatorId != null ? _userRepository.FirstOrDefault(x => x.Id == category.CreatorId).Name : "";

                listCategoryDto.Add(categoryMap);
            }

            return new ListResultDto<CategoryDto>(
               listCategoryDto
           );
        }
        public async Task<List<CategoryDto>> GetListOnlyAsync()
        {
            var categories = await _categoryRepository.GetListAsync();

            return new List<CategoryDto>(
               ObjectMapper.Map<List<Category>, List<CategoryDto>>(categories)
           );
        }
        public async Task<CategoryDto> InsertAsync(CreateCategoryDto Entity)
        {
          
                if (_categoryRepository.Any(x => x.Name.ToLower().Trim() == Entity.Name.ToLower().Trim()))
                {
                    throw new NotImplementedException("Category Exist Name !!");
                }
                if (Entity.Name.IsNullOrEmpty())
                {
                    throw new NotImplementedException("Category is not null !!");
                }
                var category = await _categoryManager.CreateAsync(
               Entity.Name.Trim()
           );

            await _categoryRepository.InsertAsync(category);
            return ObjectMapper.Map<Category, CategoryDto>(category);
        }


        public async Task UpdateAsync(CreateCategoryDto Entity, Guid Id)
        {

            var cate = await _categoryRepository.GetAsync(Id);
            var currentUserId = _tmtCurrentUser.GetId();
            if (cate.CreatorId != currentUserId && !await _userManager.CheckRoleByUserId(currentUserId, "admin"))
            {
                throw new UserFriendlyException("You not allow to edit this category !!");
            }
            
            if (_categoryRepository.Any(x => x.Name == Entity.Name))
            {
                throw new UserFriendlyException("Category Exist! Try another name!");
            }
            try
            {
                var category = await _categoryRepository.GetAsync(Id);              
                if (category == null)

                {
                    throw new EntityNotFoundException(typeof(Category), Id);
                }
                else
                {
                    category.Name = Entity.Name;
                    await _categoryRepository.UpdateAsync(category);

                }
            }
            catch
            {
                throw new UserFriendlyException("An error while try to update category !!");
            }
        }

        public async Task DeleteAsync(Guid id)
        {

            var category = await _categoryRepository.GetAsync(id);
            var currentUserId = _tmtCurrentUser.GetId();
            if(category.CreatorId!= currentUserId&& !(await _userManager.CheckRoleByUserId(currentUserId, "admin")))
            {
                throw new UserFriendlyException("You are not allow to delete this category!");
            }
            var issueExist = _isssueRepository.Any(x => x.CategoryID == id);
                if (issueExist)
                {
                    throw new UserFriendlyException("This category has an issue, cannot be delete!");
                }
                else
                {
                    await _categoryRepository.DeleteAsync(id);
                }           
        }

        public async Task<List<CategoryDto>> GetListCategoryWithIssueAsync(Guid IdProject)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            bool isProjectCreator = _projectRepository.Any(x => x.Id == IdProject && x.CreatorId == CurrentUserId);
            bool isAdmin = await _userManager.CheckRoleByUserId(CurrentUserId, "admin");

            var query = from category in _categoryRepository
                        select new
                        { category };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var categoryDtos = queryResult.Select(result =>
            {
                var categoryDto = ObjectMapper.Map<Category, CategoryDto>(result.category);
                categoryDto.IssueList = new List<StatusContractDto> { };
                if (isAdmin || isProjectCreator)
                {

                    categoryDto.count = _isssueRepository.Count(x => x.ProjectID == IdProject && x.CategoryID == categoryDto.Id);
                }
                else
                {
                    var notAdmin = from category in _categoryRepository
                                   join issue in _isssueRepository on category.Id equals issue.CategoryID
                                   join assignee in _assigneeRepository on issue.Id equals assignee.IssueID
                                   where (assignee.UserID == CurrentUserId || issue.CreatorId == CurrentUserId) && category.Id == categoryDto.Id
                                   select new
                                   { category };


                    categoryDto.count = notAdmin.Distinct().Count();
                }
                foreach (Status status in _statusRepository)
                {
                    var statusDto = ObjectMapper.Map<Status, StatusContractDto>(status);
                    statusDto.Name = status.Name;
                    statusDto.NzColor = status.NzColor;
                    if (isAdmin || isProjectCreator)
                    {
                        statusDto.CountIssue = _isssueRepository.Count(x => x.StatusID == status.Id && x.ProjectID == IdProject && x.CategoryID == categoryDto.Id);
                    }
                    else
                    {
                        var notAdmin = from issue in _isssueRepository.Where(x => x.StatusID == status.Id && x.ProjectID == IdProject && x.CategoryID == categoryDto.Id)
                                       join assignee in _assigneeRepository on issue.Id equals assignee.IssueID
                                       where (assignee.UserID == CurrentUserId || issue.CreatorId == CurrentUserId)
                                       select new { issue };

                        statusDto.CountIssue = notAdmin.Distinct().Count();
                    }
                    if (statusDto.CountIssue > 0)
                    {
                        categoryDto.IssueList.Add(statusDto);
                    }

                }

                return categoryDto;
            }).ToList();
            return categoryDtos;
        }

    }

}

