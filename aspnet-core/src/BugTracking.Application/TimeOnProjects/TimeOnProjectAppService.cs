using BugTracking.Members;
using BugTracking.Projects;
using BugTracking.UserInforTFS;
using BugTracking.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracking.TimeOnProjects
{
    public class TimeOnProjectAppService : BugTrackingAppService, ITimeOnProjectAppService
    {
        private readonly ITimeOnProjectRepository _timeOnProjectsRepository;
        private readonly TimeOnProjectManager _timeOnProjectManager;
        private readonly IMemberRepository _memberRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserInforTfsRepository _userInforTfsRepository;
        public TimeOnProjectAppService(
            ITimeOnProjectRepository timeOnProjectsRepository,
            TimeOnProjectManager timeOnProjectManager,
            IMemberRepository memberRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IUserInforTfsRepository userInforTfsRepository
            )
        {
            _timeOnProjectsRepository = timeOnProjectsRepository;
            _timeOnProjectManager = timeOnProjectManager;
            _memberRepository = memberRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _userInforTfsRepository = userInforTfsRepository;
        }
        public async Task<TimeOnProjectDto> CreateAsync(CreateTimeOnProjectDto createTimeOnProjectDto)
        {
            var timeOnProject = _timeOnProjectManager.CreateAsync(
                    createTimeOnProjectDto.ApplicationName,
                    createTimeOnProjectDto.UrlPath,
                    createTimeOnProjectDto.IsActive,
                    createTimeOnProjectDto.StartTime,
                    createTimeOnProjectDto.EndTime,
                    createTimeOnProjectDto.UniqueName
                );
            await _timeOnProjectsRepository.InsertAsync(timeOnProject);
            return ObjectMapper.Map<TimeOnProject, TimeOnProjectDto>(timeOnProject);
        }
        public async Task<TimeSpan> GetTimeBetween(Guid Id)
        {
            var timeOnProject = await _timeOnProjectsRepository.GetAsync(Id);
            var time = timeOnProject.EndTime - timeOnProject.StartTime;
            return time;
        }
        public double GetTimeEndStart(TimeOnProject timeOnProject)
        {
            return (timeOnProject.EndTime - timeOnProject.StartTime).TotalMinutes + ((timeOnProject.EndTime - timeOnProject.StartTime).TotalSeconds) / 60;
        }
        public async Task<double> GetTimeContainsAppNameOrUrlPathByMonth(AppNameOrPathDto AppNameOrPathDto)
        {
            double totalTime = 0;
            foreach(string appName in AppNameOrPathDto.AppNameOrPath)
            {
                var timesOnProject = await _timeOnProjectsRepository.GetListAsync(x => x.ApplicationName == appName && x.StartTime.Month == DateTime.UtcNow.Month);
                foreach(TimeOnProject timeOnProject in timesOnProject)
                {
                    totalTime = totalTime + GetTimeEndStart(timeOnProject);
                }
            }
            return Math.Round(totalTime,2);
        }
        public async Task<double> GetTimeContainsAppNameOrUrlPathByDate(AppNameOrPathDto AppNameOrPathDto, string date)
        {
            double totalTime = 0;
            foreach (string appName in AppNameOrPathDto.AppNameOrPath)
            {
                var timesOnProject = await _timeOnProjectsRepository.GetListAsync(x => x.ApplicationName == appName 
                && x.StartTime.Date.ToString().Contains(date)
                && x.EndTime.Date.ToString().Contains(date));

                foreach (TimeOnProject timeOnProject in timesOnProject)
                {
                    totalTime = totalTime + GetTimeEndStart(timeOnProject);
                }
            }
            return Math.Round(totalTime, 2);
        }
        public async Task<double> GetTimeContainsAppNameOrUrlPathToday(AppNameOrPathDto AppNameOrPathDto)
        {
            double totalTime = 0;
            foreach (string appName in AppNameOrPathDto.AppNameOrPath)
            {
                var timesOnProject = await _timeOnProjectsRepository.GetListAsync(x => x.ApplicationName == appName
                && x.StartTime.Date == DateTime.UtcNow.Date
                && x.EndTime.Date == DateTime.UtcNow.Date);

                foreach (TimeOnProject timeOnProject in timesOnProject)
                {
                    totalTime = totalTime + GetTimeEndStart(timeOnProject);
                }
            }
            return Math.Round(totalTime, 2);
        }
        public async Task<Dictionary<string, List<TimeOnProject>>> GetTimeToday()
        {
            Dictionary<string, List<TimeOnProject>> result = new Dictionary<string, List<TimeOnProject>>();
            var today = DateTime.UtcNow;
            var times = await _timeOnProjectsRepository.GetListAsync(x => x.StartTime.Date == today.Date && x.EndTime.Date == today.Date);
            var timesRs = times.GroupBy(x => x.ApplicationName);
            foreach(var rs in timesRs)
            {
                result.Add(rs.Key, rs.ToList());
            }
            return result;
        }
        public async Task<Dictionary<string, Dictionary<string, List<double>>>> GetTimeThisMonth()
        {
            Dictionary<string, Dictionary<string, List<double>>> result = new Dictionary<string, Dictionary<string, List<double>>>();
            var today = DateTime.UtcNow;
            var times = await _timeOnProjectsRepository.GetListAsync(x => x.StartTime.Month == today.Month && x.EndTime.Month == today.Month);
            var timesRs = times.GroupBy(x => x.EndTime.ToString("dd-MM-yyyy"));
            foreach (var rs in timesRs)
            {
                Dictionary<string, List<double>> keyValuePairs = new Dictionary<string, List<double>>();
                var listG = rs.GroupBy(x => x.ApplicationName);
                foreach(var list in listG)
                {
                    List<double> time = new List<double>();
                    var b = list.ToList();
                    foreach(var i in b)
                    {
                        time.Add(GetTimeEndStart(i));
                    }
                    var a = list.Sum(x => GetTimeEndStart(x));
                    keyValuePairs.Add(list.Key, time);
                }
                result.Add(rs.Key, keyValuePairs);
            }
            return result;
        }
        public async Task<List<MemberDto>> GetListMemberTimeOn(Guid projectId)
        {
            var query = from member in _memberRepository
                        join user in _userRepository on member.UserID equals user.Id
                        join project in _projectRepository on member.ProjectID equals project.Id
                        where project.Id == projectId
                        select new { member, project, user };

            var queryResult = await AsyncExecuter.ToListAsync(query);

            var memberDtos = queryResult.Select(x =>
            {
                var memberDto = ObjectMapper.Map<Member, MemberDto>(x.member);
                memberDto.UserName = x.user.Name;
                memberDto.ProjectName = x.project.Name;
                return memberDto;
            }).ToList();

            return memberDtos;
        }
        // Get a list of each member's online app
        public async Task<List<TimeOnProjectDto>> GetTimeOnProject(Guid projectId,DateTime timeSearch,Boolean Month)
        {
            var listMember = from project in _projectRepository
                             join member in _memberRepository on project.Id equals member.ProjectID into termMember
                             from m in termMember.DefaultIfEmpty()
                             join user in _userRepository on m.UserID equals user.Id into termUser
                             from u in termUser.DefaultIfEmpty()
                             join userTfs in _userInforTfsRepository on u.Id equals userTfs.UserId 
                             where project.Id == projectId 
                             select new {u, userTfs, project };

            listMember.Distinct();
            var queryResult = await AsyncExecuter.ToListAsync(listMember);
            int id = 100;
            var TimeOnProjectDtos = queryResult.Select(result =>
            {
                var TimeOnDto = new TimeOnProjectDto();
                TimeOnDto.Id = id;
                TimeOnDto.UniqueName = result.userTfs.UniqueName;
                TimeOnDto.NameProject = result.project.Name;
                TimeOnDto.Idproject = result.project.Id;
                TimeOnDto.NameUser = result.u.Name;
                TimeOnDto.IdUser = result.u.Id;
                TimeOnDto.Application = GetListApplication(result.userTfs.UniqueName,timeSearch,Month);
                id++;
                return TimeOnDto;             
            }).Distinct().ToList();
            return TimeOnProjectDtos;

        }
        private SumApplicationDto GetListApplication(string UniqueName,DateTime timeSearch,Boolean Month)
        {
           if(Month == false)
            {
                var listGetTime = _timeOnProjectsRepository.GetListAsync(x => x.UniqueName == UniqueName && x.StartTime.Date == timeSearch.Date);
                var listApp = from app in listGetTime.Result
                              select new ApplicationDto
                              {
                                  Name = app.ApplicationName,
                                  StartTime = app.StartTime,
                                  EndTime = app.EndTime,
                                  TotalTimeOn = (app.EndTime - app.StartTime),
                              };
                var a = new SumApplicationDto
                {
                    SumTime = listApp.Sum(x => x.TotalTimeOn.TotalMinutes),
                    App = listApp.ToList()
                };

                return a;
            }
            else
            {
                var listGetTime = _timeOnProjectsRepository.GetListAsync(x => x.UniqueName == UniqueName && x.StartTime.Month == timeSearch.Month);
                var listApp = from app in listGetTime.Result
                              select new ApplicationDto
                              {
                                  Name = app.ApplicationName,
                                  StartTime = app.StartTime,
                                  EndTime = app.EndTime,
                                  TotalTimeOn = (app.EndTime - app.StartTime),
                              };
                var a = new SumApplicationDto
                {
                    SumTime = listApp.Sum(x => x.TotalTimeOn.TotalMinutes),
                    App = listApp.ToList()
                };

                return a;
            }
        }
    }
}
