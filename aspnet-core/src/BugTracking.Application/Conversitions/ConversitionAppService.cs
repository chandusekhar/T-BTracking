
using BugTracking.Conversation;
using BugTracking.Departments;
using BugTracking.HttpClients;
using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.MemberTeams;
using BugTracking.Projects;
using BugTracking.Teams;
using BugTracking.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Conversitions
{
    public class ConversitionAppService : BugTrackingAppService, IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly ConversationManager _conversationManager;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IHttpClientService _httpClientService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMemberTeamRepository _memberTeamReoprository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IIssueRepository _issueRepository;
        public ConversitionAppService(
            IConversationRepository conversationRepository, 
            ConversationManager conversationManager,
            ITMTCurrentUser tmtCurrentUser,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IHttpClientService httpClientService,
            IMemberTeamRepository memberTeamReoprository,
            ITeamRepository teamRepository,
            IDepartmentRepository departmentRepository,
            IMemberRepository memberRepository,
            IIssueRepository issueRepository
            )
        {
            _conversationRepository = conversationRepository;
            _conversationManager = conversationManager;
            _tmtCurrentUser = tmtCurrentUser;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _httpClientService = httpClientService;
            _memberTeamReoprository = memberTeamReoprository;
            _teamRepository = teamRepository;
            _departmentRepository = departmentRepository;
            _memberRepository = memberRepository;
            _issueRepository = issueRepository;
        }

        public async Task CreateConversation(CreateConversation input)
        {
            if (!_conversationRepository.Any(x=>x.idProject == input.idProject))
            {
                var conversation = _conversationManager.CreateAsync(input.ConversationId, input.idProject);
                await _conversationRepository.InsertAsync(conversation);
            }    

        }
        public async Task<ConversationDTO> getConver(string idProject)
        {
            var conversation = await _conversationRepository.GetAsync(x => x.idProject == idProject);
            return ObjectMapper.Map<Conversation.Conversation, ConversationDTO>(conversation);

        }
        public async Task<ProjectDto> getNameProject(string idProject)
        {
            var project = await _projectRepository.GetAsync(x => x.Id.ToString() == idProject);
            return ObjectMapper.Map<Project, ProjectDto>(project);

        }

        public  bool GetCheckConversation(string idproject)
        {
            var result = _conversationRepository.Any(x => x.idProject == idproject );
            return result;
        }

        public PagedResultDto<Users.UserDto> GetListUserAll(GetListDto input)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }
            // IQueryable<AppUser> userQueryable = await _userRepository.GetQueryableAsync();
            var CurrentUserId = _tmtCurrentUser.GetId();
            var query = from user in _userRepository
                        where user.Id != CurrentUserId && user.Name != "admin" && user.Name.Contains(input.Filter.Replace(" ", ""))
                        orderby user.CreationTime descending
                        select new UserDto()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            CreationTime = user.CreationTime
                        };
            int totalCount = query.Count();
            //  query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
            var queryResult = query.ToList();
            return new PagedResultDto<Users.UserDto>(
                totalCount,
                queryResult
            );
        }
        public async Task<UserDto> getName(string id)
        {
            var query = await _userRepository.GetAsync(id);
            
            return ObjectMapper.Map<AppUser, UserDto>(query);
        }
        public async Task<InfoUserDto> GetInfoUser(string IdUser)
        {
            var listInfoUserDto = new InfoUserDto();
            if (!await _userRepository.AnyAsync(x => x.Id == IdUser))
            {
                return new InfoUserDto() { };
            }
            var teams = await _teamRepository.GetListAsync(x => _memberTeamReoprository.Any(t => t.UserID == IdUser && x.Id == t.IdTeam));
            string listTeam = "";
            listTeam = String.Join(" / ", teams.Select(x => x.Name));
            var departments = await _departmentRepository.GetListAsync(x => _teamRepository.Any(t => t.IdDepartment == x.Id && _memberTeamReoprository.Any(m => m.UserID == IdUser && t.Id == m.IdTeam)));
            string listDepart = "";
            listDepart = String.Join(" / ", departments.Select(x => x.Name));

            //var user = from userQue in _userRepository
            //           where user.Id == userId
            //           select new { user };
            var user = await _userRepository.GetAsync(IdUser);
            listInfoUserDto.name = user?.Name;
            listInfoUserDto.email = user?.Email;
            listInfoUserDto.phone = user?.PhoneNumber;
            listInfoUserDto.teams = listTeam;
            listInfoUserDto.departments = listDepart;
            return listInfoUserDto;
        }
        public async Task<List<TagProjectDTO>> getListProjectTag(string projectId)
        {
            var userId = _tmtCurrentUser.Id;
            List<TagProjectDTO> tagProjectDTOs;

            if(projectId == "" || projectId == "null")
            {
                var projects = await _projectRepository.GetListAsync(x => _memberRepository.Any(m => m.UserID == userId && m.ProjectID == x.Id));
                tagProjectDTOs = ObjectMapper.Map<List<Project>, List<TagProjectDTO>>(projects);
            }
            else
            {
                var query = from project in _projectRepository
                            join member in _memberRepository on project.Id equals member.ProjectID
                            join issue in _issueRepository on project.Id equals issue.ProjectID
                            where member.UserID == userId && (!projectId.IsNullOrEmpty() ? project.Id.ToString() == projectId : true)
                            select new TagProjectDTO
                            {
                                Name = issue.Name,
                                Id = issue.Id
                            };
                tagProjectDTOs = query.Distinct().ToList();
            }



            return tagProjectDTOs;
        }
        //public async Task<List<TagProjectDTO>> getListIssueTag(string idProject)
        //{
        //    if (idProject.IsNullOrWhiteSpace() || idProject == "null")
        //    {
        //        idProject = "";
        //    }
        //    var userId = _tmtCurrentUser.Id;
        //    var query = from project in _projectRepository
        //                join member in _memberRepository on project.Id equals member.ProjectID
        //                join issue in _issueRepository on project.Id equals issue.ProjectID
        //                where member.UserID == userId && (!idProject.IsNullOrEmpty() ? project.Id.ToString() == idProject : true)
        //                select new TagProjectDTO
        //                {
        //                    name = project.Name,
        //                    id = project.Id.ToString()
        //                };
        //    return query.Distinct().ToList();
        //}
    }

}
