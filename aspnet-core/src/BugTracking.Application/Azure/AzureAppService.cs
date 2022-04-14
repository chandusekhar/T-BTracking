
using BugTracking.Assignees;
using BugTracking.Attachments;
using BugTracking.Azures;
using BugTracking.Categories;
using BugTracking.Comments;
using BugTracking.ConditionTypeWit;
using BugTracking.DetailAttachment;
using BugTracking.Follows;
using BugTracking.Hub;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Notifications;
using BugTracking.PriorityEnum;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.UserInforTFS;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using static BugTracking.Azures.DataAzureTFS;

namespace BugTracking.Azure
{
    public class AzureAppService : BugTrackingAppService, IAzureAppService
    {
        private readonly IAzureRepository _azureRepository;
        private readonly AzureManager _azureManager;
        private readonly UserManager _userManager;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly ICommentRepository _commentRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly AssigneeService _assigneeService;
        private readonly AssigneeManager _assigneeManager;
        private readonly CommentManager _commentManager;
        private IHubContext<SignalR> _hub;
        private readonly NotificationsAppService _notificationsAppService;
        private readonly FollowManager _followManager;
        private readonly IFollowRepository _followRepository;
        private readonly IssueManager _issueManager;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IUserInforTfsRepository _userInforTfsRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IssueAppService _issueAppService;
        private readonly MemberManager _memberManager;
        public AzureAppService(IAzureRepository azureRepository, 
            AzureManager azureManager, 
            UserManager userManager, 
            ITMTCurrentUser tMTCurrentUser,
            ICommentRepository commentRepository, 
            IIssueRepository issueRepository, 
            IProjectRepository projectRepository, 
            IStatusRepository statusRepository,
            ICategoryRepository categoryRepository, 
            IUserRepository userRepository, IAssigneeRepository assigneeRepository, 
            AssigneeService assigneeService,
            AssigneeManager assigneeManager, 
            CommentManager commentManager, 
            IHubContext<SignalR> hub, 
            NotificationsAppService notificationsAppService,
            FollowManager followManager, 
            IFollowRepository followRepository, 
            IssueManager issueManager, 
            IAttachmentRepository attachmentRepository,
            IUserInforTfsRepository userInforTfsRepository, 
            IMemberRepository memberRepository,
            IssueAppService issueAppService,
            MemberManager memberManager)
        {
            _azureRepository = azureRepository;
            _azureManager = azureManager;
            _userManager = userManager;
            _tmtCurrentUser = tMTCurrentUser;
            _commentRepository = commentRepository;
            _issueRepository = issueRepository;
            _projectRepository = projectRepository;
            _statusRepository = statusRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _assigneeRepository = assigneeRepository;
            _assigneeService = assigneeService;
            _assigneeManager = assigneeManager;
            _commentManager = commentManager;
            _hub = hub;
            _notificationsAppService = notificationsAppService;
            _followManager = followManager;
            _followRepository = followRepository;
            _issueManager = issueManager;
            _attachmentRepository = attachmentRepository;
            _userInforTfsRepository = userInforTfsRepository;
            _memberRepository = memberRepository;
            _issueAppService = issueAppService;
            _memberManager = memberManager;
        }
        public string Projects = "/_apis/projects?api-version=5.0";
        public string Subscription = "/_apis/hooks/subscriptions?api-version=6.1-preview.1";
        public string templateTypeId = "adcc42ab-9882-485e-a3ed-7678f01f66bc";//Agile
        public string sourceControlType = "Git";
        public string SYSTMT = "SYSTMT\\";
        public string catchUpdatedApiLink = "https://t-btracking.tpos.dev/api/app/azure/catch-update-changes-from-tfs";
        public string catchCreatedApiLink = "https://t-btracking.tpos.dev/api/app/azure/issue-from-wit";
        public string catchDeletedApilink = "https://t-btracking.tpos.dev/api/app/azure/catch-delete-wit";

        public async Task CreateHost(CreateUpdateHostDto input)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            if (!await _userManager.CheckRoleByUserId(CurrentUserId, "admin"))
            {
                throw new UserFriendlyException("You're not allow to create!!");
            }
            if (_azureRepository.Any())
            {
                throw new UserFriendlyException("Host Already Exist!!");
            }
            if (!input.Host.StartsWith("https://"))
            {
                input.Host = "https://" + input.Host;
            }
            var azure = _azureManager.CreateAsync(
                input.Host,
                input.Collection
            );
            await _azureRepository.InsertAsync(azure);
        }
        public async Task UpdateHost(CreateUpdateHostDto input, Guid Id)
        {
            var currentUserId = _tmtCurrentUser.GetId();
            if (!await _userManager.CheckRoleByUserId(currentUserId, "admin"))
            {
                throw new UserFriendlyException("You're not allow!!!");
            }
            try
            {
                var azure = await _azureRepository.GetAsync(Id);
                if (azure == null)

                {
                    throw new EntityNotFoundException();
                }
                else
                {
                    if (!input.Host.StartsWith("https://"))
                    {
                        input.Host = "https://" + input.Host;
                    }
                    azure.Host = input.Host;
                    azure.Collection = input.Collection;
                    await _azureRepository.UpdateAsync(azure);
                }
            }
            catch
            {
                throw new UserFriendlyException("An error while try to update !!");
            }
        }
        //Get Host, Collection, PAT from DB to connect to TFS
        public AzureDto GetHost()
        {
            if (_azureRepository.Any())
            {
                var Host = _azureRepository.FirstOrDefault();
                return ObjectMapper.Map<Azures.Azure, AzureDto>(Host);
            }
            else return null;
        }
        public void Authorize(HttpClient client, string CurrentUserId)
        {
            string PAT = "";
            var UserInfor = _userInforTfsRepository.FirstOrDefault(x => x.UserId == CurrentUserId);
            if (UserInfor == null)
            {
                throw new UserFriendlyException("Please Update your Tfs Information or check your Personal Access Token");
            }
            else PAT = UserInfor.PAT;
            client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", PAT))));
        }
        public void AuthorizeWithPat(HttpClient client, string PAT)
        {
            client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", PAT))));
        }
        private async Task<string> GetHttpClient(string url, string CurrentUserId)
        {
            var Host = GetHost();
            using (HttpClient client = new HttpClient())
            {
                Authorize(client, CurrentUserId);
                using (HttpResponseMessage response = await client.GetAsync(
                            Host.Host + "/" + Host.Collection + url))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
        }

        private async Task<string> GetHttpWithUrlClient(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
        }

        private async Task<string> PostAsJsonHttpClient<T>(string url, T classT, string CurrentUserId)
        {
            var Host = GetHost();
            using (HttpClient client = new HttpClient())
            {
                Authorize(client, CurrentUserId);
                using (HttpResponseMessage response = await client.PostAsJsonAsync(
                            Host.Host + "/" + Host.Collection + url, classT))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new UserFriendlyException(await response.Content.ReadAsStringAsync());
                    }
                    return responseBody;
                }
            }
        }
        private async Task<string> MethodHttpClient(string url, string json, string method, string CurrentUserId)
        {
            var Host = GetHost();
            HttpClientHandler _httpclienthndlr = new HttpClientHandler();
            using (HttpClient client = new HttpClient(_httpclienthndlr))
            {
                Authorize(client, CurrentUserId);
                var request = new HttpRequestMessage(new HttpMethod(method), Host.Host + "/" + Host.Collection + url)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json-patch+json")
                };
                HttpResponseMessage responseMessage = client.SendAsync(request).Result;
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new UserFriendlyException(await responseMessage.Content.ReadAsStringAsync());
                }
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
        private async Task PatchHttpClient(string url, object value, Type type, string CurrentUserId)
        {
            var Host = GetHost();
            using (HttpClient client = new HttpClient())
            {
                Authorize(client, CurrentUserId);
                using (HttpResponseMessage response = await client.PatchAsync(
                                            Host.Host + "/" + Host.Collection + url, new ObjectContent(type, value, new JsonMediaTypeFormatter())))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new UserFriendlyException(await response.Content.ReadAsStringAsync());
                    }
                }
            }

        }
        private async Task DeleteHttpClient(string url, string CurrentUserId)
        {
            var Host = GetHost();
            using (HttpClient client = new HttpClient())
            {
                Authorize(client, CurrentUserId);
                using (HttpResponseMessage response = await client.DeleteAsync(
                                            Host.Host + "/" + Host.Collection + url))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new UserFriendlyException(await response.Content.ReadAsStringAsync());
                    }
                }
            }

        }

        public async Task<string> GetProcess(string CurrentUserId)
        {
            var rs = await GetHttpClient("/_apis/process/processes?api-version=6.1-preview.1", CurrentUserId);
            return rs;
        }
        public async Task<string> GetProjectByPAT(string PAT)
        {
            var Host = GetHost();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", "", PAT))));
                using (HttpResponseMessage response = await client.GetAsync(
                                Host.Host + "/" + Host.Collection + Projects))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new UserFriendlyException("Please Update your Tfs Information or check your Personal Access Token");
                    }
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
        }
        public async Task<string> GetTMTProjects(string CurrentUserId)
        {
            var rs = await GetHttpClient(Projects, CurrentUserId);
            return rs;
        }
        public async Task<object> GetProjectTFS(string id, string CurrentUserId)
        {
            var rs = await GetHttpClient("/_apis/projects/" + id + "?api-version=6.1-preview.4", CurrentUserId);
            return rs;
        }
        public async Task<string> GetSubctiptions(string projectName, string currentUserId)
        {
            var rs = await GetHttpClient("/" + projectName + "/_apis/hooks/subscriptions?api-version=6.1-preview.1", currentUserId);
            return rs;
        }

        //"workitem.created"
        public async Task CreateSubscription(Guid projectId, string UrlHost, string eventType, string CurrentUserId)
        {
            var webHookJson = new WebHookJson() { };
            webHookJson.eventType = eventType;
            var publisherInputs = new publisherInputs() { };
            publisherInputs.projectId = projectId.ToString();
            webHookJson.publisherInputs = publisherInputs;
            var consumerInputs = new consumerInputs() { };
            consumerInputs.url = UrlHost;
            webHookJson.consumerInputs = consumerInputs;
            await PostAsJsonHttpClient(Subscription, webHookJson, CurrentUserId);
        }
        public async Task CreateProject(string name, string CurrentUserId, string des)
        {
            DataProjectTfs projectJson = new DataProjectTfs() { };
            projectJson.name = name;
            projectJson.description = des;
            var processTemplate = new processTemplate() { };
            processTemplate.templateTypeId = templateTypeId;
            var versioncontrol = new versioncontrol() { };
            versioncontrol.sourceControlType = sourceControlType;
            var capabilities = new capabilities() { };
            capabilities.versioncontrol = versioncontrol;
            capabilities.processTemplate = processTemplate;
            projectJson.capabilities = capabilities;
            await PostAsJsonHttpClient("/_apis/projects?api-version=6.1-preview.4", projectJson, CurrentUserId);
        }
        public async Task<IdentitySelf> getProfile()
        {
            var Host = _azureRepository.FirstOrDefault();
            WindowsCredential windowsCredential = new WindowsCredential();
            VssClientCredentials vssClientCredentials = new VssClientCredentials(windowsCredential);
            IdentityHttpClient identityHttpClient = new IdentityHttpClient(new Uri(Host.Host), vssClientCredentials);
            var rs = await identityHttpClient.GetIdentitySelfAsync();
            return rs;
        }

        [Authorize("BugTracking.Users")]
        public async Task SyncProject(dataSyncProject dataSyncProject)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var project = await _projectRepository.GetAsync(x => x.Name == dataSyncProject.name && x.IsDeleted == false);
            try
            {
                if (project.CreatorId != CurrentUserId && await _userManager.CheckRoleByUserId(CurrentUserId, "admin") == false)
                {
                    throw new UserFriendlyException("You are not allow to sync!");
                }

                if (!_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
                {
                    throw new UserFriendlyException("You need to update your profile to sync project to TFS! Personal Access Token is required");
                }

                if (project.WitType != dataSyncProject.witType)
                {
                    project.WitType = dataSyncProject.witType;
                    if (dataSyncProject.witType == "Bug") project.NzColor = "#DC143C";
                    if (dataSyncProject.witType == "Epic") project.NzColor = "#F50";
                    if (dataSyncProject.witType == "Feature") project.NzColor = "#9400D3";
                    if (dataSyncProject.witType == "Issue") project.NzColor = "#483D8B";
                    if (dataSyncProject.witType == "Task") project.NzColor = "#DAA520";
                    if (dataSyncProject.witType == "Test Case") project.NzColor = "#808080";
                    if (dataSyncProject.witType == "User Story") project.NzColor = "#1E90FF";

                    await _projectRepository.UpdateAsync(project);
                }

                await CreateProject(dataSyncProject.name, CurrentUserId, dataSyncProject.description);

                do
                {
                    await GetProjectsFromTFS(dataSyncProject.name, CurrentUserId);
                }
                while (await GetProjectsFromTFS(dataSyncProject.name, CurrentUserId) == false);

                //if (_memberRepository.Count(x => x.ProjectID == project.Id) > 0)
                //{
                //    var members = await _memberRepository.GetListAsync(x => x.ProjectID == project.Id);
                //    foreach(var member in members)
                //    {
                //        if(_userInforTfsRepository.Any(x=>x.UserId == member.UserID))
                //        {
                //            if (member.UserID == CurrentUserId)
                //            {
                //                continue;
                //            }
                //            await CreateMember(project.Id, _userInforTfsRepository.FirstOrDefault(x => x.UserId == member.UserID).UniqueName);
                //        }   
                //    }

                //}

                await CreateSubscription(project.ProjectIdTFS, catchCreatedApiLink, "workitem.created", CurrentUserId);
                await CreateSubscription(project.ProjectIdTFS, catchUpdatedApiLink, "workitem.updated", CurrentUserId);
                await CreateSubscription(project.ProjectIdTFS, catchDeletedApilink, "workitem.deleted", CurrentUserId);

                if (dataSyncProject.listIssueSync.Any())
                {
                    var issuesDto = dataSyncProject.listIssueSync.OrderBy(x => x.IdParent);
                    foreach (var issueDto in issuesDto)
                    {
                        var assignees = await _assigneeRepository.GetListAsync(x => x.IssueID == issueDto.Id);
                        await CreateWIT(issueDto.Id,
                            project.Name,
                            issueDto.Name,
                            issueDto.Description,
                            ((int)Enum.Parse(typeof(Priority),
                            issueDto.Priority.ToString()) + 1).ToString(),
                            assignees.Select(x => x.UserID).ToList(),
                            CurrentUserId,
                            issueDto.IdParent);
                        //var comments = await _commentRepository.GetListAsync(x => x.IssueID == issue.Id);

                        //foreach(var comment in comments)
                        //{
                        //    if(_userInforTfsRepository.Any(x=>x.UserId == comment.UserID))
                        //    {
                        //        await InsertComment(comment.Id, comment.IssueID, comment.Content, project.Name, comment.UserID);
                        //    }
                        //}
                    }
                    //var openStatus = await _statusRepository.GetAsync(x => x.Name == "Open");
                    //foreach(var issueDto in issuesDto.Where(x=>x.StatusID != openStatus.Id && x.IdWIT != 0))
                    //{
                    //    await UpdateState(issueDto.Id, issueDto.StatusID, CurrentUserId);
                    //}
                }
            }
            catch
            {
                await DeleteProject(project.Id, CurrentUserId);
                throw new UserFriendlyException("An error when sync project to TFS. Check your Access Token and Try again!");
            }
        }

        //update ProjectIdTFS when find projectTfs == project(name)
        public async Task<bool> GetProjectsFromTFS(string name, string CurrentUserId)
        {
            var rs = false;
            var project = await _projectRepository.GetAsync(x => x.Name == name && x.IsDeleted == false);
            var projectsTfs = JsonConvert.DeserializeObject<ProjectsTFS>(await GetTMTProjects(CurrentUserId));

            if (projectsTfs.value.Any(x => x.name == name))
            {
                project.ProjectIdTFS = Guid.Parse(projectsTfs.value.FirstOrDefault(x => x.name == name).id);
                await _projectRepository.UpdateAsync(project);
                rs = true;
            }

            return rs;
        }
        public async Task UpdateProject(Guid id, string name, string CurrentUserId, string des)
        {
            DataProjectTfs projectJson = new DataProjectTfs() { };
            projectJson.name = name;
            projectJson.description = des;
            await PatchHttpClient("/_apis/projects/" + id + "?api-version=6.1-preview.4", projectJson, typeof(DataProjectTfs), CurrentUserId);
        }
        public async Task DeleteProject(Guid id, string CurrentUserId)
        {
            var project = await _projectRepository.GetAsync(id);
            var issues = await _issueRepository.GetListAsync(x => x.ProjectID == id);
            issues.ForEach(i => i.IdWIT = 0);
            issues.ForEach(i => i.Rev = 0);
            await _issueRepository.UpdateManyAsync(issues);
            await DeleteHttpClient("/_apis/projects/" + project.ProjectIdTFS + "?api-version=6.1-preview.4", CurrentUserId);
            project.ProjectIdTFS = Guid.Empty;
            await _projectRepository.UpdateAsync(project);
        }
        public async Task UpdateRev(Guid Id, int Rev)
        {
            var issue = await _issueRepository.GetAsync(Id);
            issue.Rev = Rev;
            await _issueRepository.UpdateAsync(issue);
        }
        private UserInforTFS.UserInforTfs GetUserEqualToUserAssigneeTfsAsync(string userAssigneeTfs, string displayName)
        {
            UserInforTFS.UserInforTfs userInforTfs;
            if(_userInforTfsRepository.Any(x=>x.UniqueName.Replace("\\", "") == userAssigneeTfs.Replace("\\", "")))
            {
                if (_userInforTfsRepository.Any(x => x.UniqueName.Replace("\\", "") == userAssigneeTfs.Replace("\\", "")) && _userRepository.Any(x => x.Name == displayName))
                {
                    var user = _userRepository.FirstOrDefault(x => x.Name == displayName);
                    userInforTfs = _userInforTfsRepository.FirstOrDefault(x => x.UniqueName.Replace("\\", "").Contains(userAssigneeTfs.Replace("\\", "")) && x.UserId == user.Id);
                }
                else
                {
                    userInforTfs = _userInforTfsRepository.FirstOrDefault(x => x.UniqueName.Replace("\\", "").Contains(userAssigneeTfs.Replace("\\", "")));
                }
            }
            else
            {
                if(_userRepository.Any(x => x.Name == displayName))
                {
                    userInforTfs = _userInforTfsRepository.FirstOrDefault(x => x.UserId == _userRepository.FirstOrDefault(x => x.Name == displayName).Id);
                }
                else
                {
                    userInforTfs = null;
                }
            }
            return userInforTfs;
        }
        private UserInforTFS.UserInforTfs GetUserEqualToUserChangeTfsAsync(string uniqueName, string displayName)
        {
            UserInforTFS.UserInforTfs userInforTfs;
            if (_userInforTfsRepository.Any(x => x.UniqueName.Replace("\\", "") == uniqueName.Replace("\\", "")))
            {
                if (_userInforTfsRepository.Any(x => x.UniqueName.Replace("\\", "") == uniqueName.Replace("\\", "")) && _userRepository.Any(x => x.Name == displayName))
                {
                    var user = _userRepository.FirstOrDefault(x => x.Name == displayName);
                    userInforTfs = _userInforTfsRepository.FirstOrDefault(x => x.UniqueName.Replace("\\", "").Contains(uniqueName.Replace("\\", "")) && x.UserId == user.Id);
                }
                else
                {
                    userInforTfs = _userInforTfsRepository.FirstOrDefault(x => x.UniqueName.Replace("\\", "").Contains(uniqueName.Replace("\\", "")));
                }
            }
            else
            {
                if(_userRepository.Any(x => x.Name == displayName))
                {
                    userInforTfs = _userInforTfsRepository.FirstOrDefault(i => i.UserId == _userRepository.FirstOrDefault(x => x.Name == displayName).Id);
                }
                else
                {
                    userInforTfs = null;
                }
            }
            return userInforTfs;
        }
        private string GetValueDictionaryKey(Dictionary<string, dynamic> keyValuePairs, string key)
        {
            return keyValuePairs.FirstOrDefault(x => x.Key.Contains(key)).Value.ToString();
        }
        private object GetValueIDictionaryKey(IDictionary<string, object> keyValuePairs, string key)
        {
            return keyValuePairs.FirstOrDefault(x => x.Key.Contains(key)).Value;
        }
        public async Task CatchUpdateChangesFromTfs(UpdateChangesFromTfs UpdateChangesFromTfs)
        {
            if (_issueRepository.Any(x => x.IdWIT == UpdateChangesFromTfs.resource.workItemId))
            {
                if (_issueRepository.FirstOrDefault(x => x.IdWIT == UpdateChangesFromTfs.resource.workItemId).Rev < UpdateChangesFromTfs.resource.rev)
                {
                    var fields = UpdateChangesFromTfs.resource.fields;
                    if (fields != null)
                    {
                        List<string> guids = new List<string>();
                        var issue = await _issueRepository.GetAsync(x => x.IdWIT == UpdateChangesFromTfs.resource.workItemId);
                        var project = await _projectRepository.GetAsync(x => x.Id == issue.ProjectID);
                        var dictionaryFields = UpdateChangesFromTfs.resource.revision.fields;

                        if (fields.ContainsKey("System.Title"))
                        {
                            issue.Name = GetValueDictionaryKey(dictionaryFields, "Title");
                        }

                        if (dictionaryFields.ContainsKey("System.Description"))
                        {
                            issue.Description = GetValueDictionaryKey(dictionaryFields, "Description");
                        }

                        if (fields.ContainsKey("System.Tags"))
                        {
                            string tags = dictionaryFields.FirstOrDefault(x => x.Key.Contains("Tags")).Value.ToString();
                            if (tags == string.Empty)
                            {
                                var assignees = await _assigneeRepository.GetListAsync(x => x.IssueID == issue.Id);
                                if (assignees.Count > 1)
                                {
                                    for (var i = 1; i < assignees.Count; i++)
                                    {
                                        await _assigneeRepository.DeleteAsync(assignees[i]);
                                    }
                                }
                            }
                            else
                            {
                                if (!tags.Contains(";"))
                                {
                                    var assignees = await _assigneeRepository.GetListAsync(x => x.IssueID == issue.Id);
                                    if (assignees.Count > 1)
                                    {
                                        for (var i = 1; i < assignees.Count; i++)
                                        {
                                            var user = _userRepository.FirstOrDefault(x => x.Name == tags);
                                            if (assignees[i].UserID != user.Id)
                                            {
                                                await _assigneeRepository.DeleteAsync(assignees[i]);
                                                var follow = await _followRepository.GetListAsync(x => x.IssueID == issue.Id && x.UserID == assignees[i].UserID);
                                                await _followRepository.DeleteManyAsync(follow);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var tagSplit = tags.Split(";");
                                    List<string> assigneesString = new List<string>();
                                    var assignees = _assigneeRepository.Where(x => x.IssueID == issue.Id).Select(x => x.UserID).ToList();
                                    assigneesString.Add(assignees.FirstOrDefault());
                                    for (var j = 0; j < tagSplit.Length; j++)
                                    {
                                        var user = _userRepository.FirstOrDefault(x => x.Name.Trim() == tagSplit[j].Trim());
                                        if (user != null)
                                        {
                                            assigneesString.Add(user.Id);
                                        }
                                    }
                                    var except = assignees.Except(assigneesString).ToList();
                                    if (except.Count > 0)
                                    {
                                        for (var i = 0; i < except.Count; i++)
                                        {
                                            await _assigneeRepository.DeleteAsync(x => x.UserID == except[i] && x.IssueID == issue.Id);
                                            var follow = await _followRepository.GetListAsync(x => x.IssueID == issue.Id && x.UserID == except[i]);
                                            await _followRepository.DeleteManyAsync(follow);
                                        }
                                    }
                                }
                            }
                        }

                        if (fields.ContainsKey("Microsoft.VSTS.Common.Priority"))
                        {
                            string PriorityName = Enum.GetName(typeof(Priority), int.Parse(GetValueDictionaryKey(dictionaryFields, "Priority")) - 1);
                            issue.Priority = (Priority)Enum.Parse(typeof(Priority), PriorityName);
                        }

                        if (fields.ContainsKey("System.State"))
                        {
                            string state = GetValueDictionaryKey(dictionaryFields, "State");
                            string witType = GetValueDictionaryKey(dictionaryFields, "WorkItemType");
                            var states = await GetStateAsync(project.Name, witType, 
                                (_userInforTfsRepository.Any(x=>x.UserId == project.CreatorId)) ? project.CreatorId : _userInforTfsRepository.FirstOrDefault().UserId);
                            var anyStateEqualStatus = _statusRepository.FirstOrDefault(x => x.Name == state);
                            if (anyStateEqualStatus != null)
                            {
                                issue.StatusID = anyStateEqualStatus.Id;
                                if (state == "Closed")
                                {
                                    issue.FinishDate = DateTime.Now;
                                }

                            }
                            else
                            {
                                var indexState = states.value.FindIndex(x => x.name == state);
                                var statusCount = _statusRepository.Count();

                                if (indexState <= statusCount)
                                {
                                    issue.StatusID = _statusRepository.FirstOrDefault(x => x.CurrentIndex == indexState).Id;
                                }
                                else
                                {
                                    issue.StatusID = _statusRepository.FirstOrDefault(x => x.CurrentIndex == statusCount - 1).Id;
                                }
                            }
                        }

                        if (dictionaryFields.ContainsKey("System.AssignedTo"))
                        {
                            string userAssigneeTfs = GetValueDictionaryKey(dictionaryFields, "AssignedTo");
                            string displayNameUserAssignee = userAssigneeTfs;

                            //get unique name
                            int index = userAssigneeTfs.LastIndexOf("_") + 1;
                            userAssigneeTfs = userAssigneeTfs.Remove(0, index).Replace(">", "");

                            //get display name
                            int indexName = displayNameUserAssignee.LastIndexOf("<") - 1;
                            var userNameAssigneeTo = displayNameUserAssignee.Remove(indexName, displayNameUserAssignee.Length - indexName);

                            if (_userInforTfsRepository.Any(x => x.UniqueName.Contains(userAssigneeTfs)))
                            {
                                UserInforTFS.UserInforTfs userInforTfs = GetUserEqualToUserAssigneeTfsAsync(userAssigneeTfs, userNameAssigneeTo);
                                if (!_assigneeRepository.Any(x => x.UserID == userInforTfs.UserId && x.IssueID == issue.Id))
                                {
                                    var assignee = _assigneeManager.CreateAsync(issue.Id, userInforTfs.UserId);
                                    guids.Add(userInforTfs.UserId);

                                    var assigneed = await _assigneeRepository.InsertAsync(assignee);
                                    var follow = _followManager.CreateAsync(assigneed.IssueID, assigneed.UserID);

                                    await _notificationsAppService.InsertAsync(assigneed.IssueID, assignee.UserID,
                                        UpdateChangesFromTfs.resource.revisedBy.displayName + " has been assigneed you into an issue: " + issue.Name);

                                    await _followRepository.InsertAsync(follow);
                                }
                                else
                                {
                                    guids.Add(string.Empty);
                                }
                            }
                            else
                            {
                                guids.Add(string.Empty);
                            }
                        }
                        else
                        {
                            if (fields.ContainsKey("System.AssignedTo"))
                            {
                                var userAssigneeTfs = GetValueDictionaryKey(fields, "AssignedTo");
                                var converted = JsonConvert.DeserializeObject<valueObjectChange>(userAssigneeTfs);
                                int index = converted.oldValue.LastIndexOf("_") + 1;
                                var user = converted.oldValue.Remove(0, index).Replace(">", "");
                                if (_userInforTfsRepository.Any(x => x.UniqueName.Contains(user)))
                                {
                                    var assignees = _assigneeRepository.Where(x => x.IssueID == issue.Id).Select(x => x.UserID).ToList();
                                    var users = _userInforTfsRepository.Where(x => x.UniqueName.Contains(user)).Select(x => x.UserId).ToList();
                                    var assigneesDeleted = users.Intersect(assignees).ToList();
                                    if (assigneesDeleted.Any())
                                    {
                                        var assignee = _assigneeRepository.FirstOrDefault(x => x.UserID == assigneesDeleted[0] && x.IssueID == issue.Id);
                                        guids.Add(assignee.UserID);
                                        await _assigneeRepository.DeleteAsync(assignee);
                                        await _notificationsAppService.InsertAsync(assignee.IssueID, assignee.UserID,
                                        UpdateChangesFromTfs.resource.revisedBy.displayName + " has been removed: " + _userRepository.FirstOrDefault(x => x.Id == assignee.UserID).Name);
                                        await _followRepository.DeleteAsync(x => x.IssueID == issue.Id && x.UserID == assignee.UserID);
                                    }
                                    else
                                    {
                                        guids.Add(string.Empty);
                                    }
                                }
                                else
                                {
                                    guids.Add(string.Empty);
                                }
                            }
                            else
                            {
                                guids.Add(string.Empty);
                            }
                        }
                        if (UpdateChangesFromTfs.resource.revision.commentVersionRef != null)
                        {
                            //get unique user
                            var userTfs = UpdateChangesFromTfs.resource.revisedBy.uniqueName.Replace("\\", "");
                            //if user in for tfs exist
                            if (_userInforTfsRepository.Any(x => x.UniqueName.Replace("\\", "").Contains(userTfs)))
                            {
                                UserInforTFS.UserInforTfs userInforTfs = GetUserEqualToUserChangeTfsAsync(userTfs, UpdateChangesFromTfs.resource.revisedBy.displayName);
                                //check if comment of wit tfs exist System.CommentCount => create or delete
                                if (fields.ContainsKey("System.CommentCount"))
                                {
                                    var commentCount = GetValueDictionaryKey(fields, "CommentCount");
                                    var commentCountConverted = JsonConvert.DeserializeObject<valueObjectChange>(commentCount);
                                    //newValue > oldValue => create 
                                    if (int.Parse(commentCountConverted.newValue) > int.Parse(commentCountConverted.oldValue))
                                    {
                                        if (!_commentRepository.Any(x => x.WitCommentId == UpdateChangesFromTfs.resource.revision.commentVersionRef.commentId))
                                        {
                                            var comment = _commentManager.Create(issue.Id, userInforTfs.UserId, GetValueDictionaryKey(dictionaryFields, "History"),
                                            UpdateChangesFromTfs.resource.revision.commentVersionRef.commentId);
                                            Comments.Comment commentCreated = await _commentRepository.InsertAsync(comment);
                                        }
                                    }
                                    //newValue < oldValue => delete 
                                    else if (int.Parse(commentCountConverted.newValue) < int.Parse(commentCountConverted.oldValue))
                                    {
                                        if (_commentRepository.Any(x => x.WitCommentId == UpdateChangesFromTfs.resource.revision.commentVersionRef.commentId))
                                        {
                                            var comment = await _commentRepository.GetAsync(x => x.WitCommentId == UpdateChangesFromTfs.resource.revision.commentVersionRef.commentId);
                                            await _commentRepository.DeleteAsync(comment);
                                        }
                                    }
                                }
                                //else not exist => update 
                                else
                                {
                                    if (_commentRepository.Any(x => x.WitCommentId == UpdateChangesFromTfs.resource.revision.commentVersionRef.commentId))
                                    {
                                        var commentWIT = await GetCommentById(project.ProjectIdTFS.ToString(),
                                            issue.IdWIT,
                                            UpdateChangesFromTfs.resource.revision.commentVersionRef.commentId,
                                            userInforTfs.UserId);

                                        var commentConverted = JsonConvert.DeserializeObject<CommentWit>(commentWIT);
                                        var comment = await _commentRepository.GetAsync(x => x.WitCommentId == commentConverted.id);
                                        if (comment.Content != commentConverted.text)
                                        {
                                            comment.Content = commentConverted.text;
                                            await _commentRepository.UpdateAsync(comment);
                                        }
                                    }
                                }

                            }
                        }
                        if (issue.Rev != UpdateChangesFromTfs.resource.rev)
                        {
                            issue.Rev = UpdateChangesFromTfs.resource.rev;
                            var updated = await _issueRepository.UpdateAsync(issue);
                            guids.Add(updated.ProjectID.ToString());
                            guids.Add(updated.Id.ToString());
                            await _hub.Clients.All.SendAsync("ChangesFromTfs", guids);
                        }
                    }
                }
            }
        }
        public async Task<string> GetCommentById(string projectId, int WitId, int commentId, string currentUserId)
        {
            var rs = await GetHttpClient("/" + projectId + "/_apis/wit/workItems/" + WitId + "/comments/" + commentId + "?api-version=6.1-preview.3", currentUserId);
            return rs;
        }
        public async Task CreateIssueFromWit(UpdateChangesFromTfs UpdateChangesFromTfs)
        {
            if (!_issueRepository.Any(x => x.IdWIT == UpdateChangesFromTfs.resource.id))
            {
                var dictionaryFields = UpdateChangesFromTfs.resource.fields;
                var userCreatedByTfs = GetValueDictionaryKey(dictionaryFields, "CreatedBy");
                int indexCreatedBy = userCreatedByTfs.LastIndexOf("_") + 1;
                int indexNameCreatedBy = userCreatedByTfs.LastIndexOf("<") - 1;
                //get display name user change
                var userNameCreatedBy = userCreatedByTfs.Remove(indexNameCreatedBy, userCreatedByTfs.Length - indexNameCreatedBy);
                //get unique name user change
                var userUniqueNameCreatedBy = userCreatedByTfs.Remove(0, indexCreatedBy).Replace(">", "");

                List<string> guids = new List<string>();
                string name = GetValueDictionaryKey(dictionaryFields, "Title");
                string teamProject = GetValueDictionaryKey(dictionaryFields, "TeamProject");
                var project = await _projectRepository.GetAsync(x => x.Name == teamProject);
                var status = await _statusRepository.GetAsync(x => x.Name == "Open");
                Priority priority = Priority.Undefined;
                Guid issueParent = Guid.Empty;
                string description = "";

                if (dictionaryFields.ContainsKey("Microsoft.VSTS.TCM.ReproSteps"))
                {
                    description = GetValueDictionaryKey(dictionaryFields, "ReproSteps");
                }
                else if (dictionaryFields.ContainsKey("System.Description"))
                {
                    description = GetValueDictionaryKey(dictionaryFields, "Description");
                }


                if (dictionaryFields.ContainsKey("System.Parent"))
                {
                    string idWitParent = GetValueDictionaryKey(dictionaryFields, "Parent");
                    var issueByIdWitParent = await _issueRepository.GetAsync(x => x.IdWIT == int.Parse(idWitParent));
                    issueParent = issueByIdWitParent.Id;
                }

                if (dictionaryFields.ContainsKey("Microsoft.VSTS.Common.Priority"))
                {
                    string PriorityName = Enum.GetName(typeof(Priority), int.Parse(GetValueDictionaryKey(dictionaryFields, "Priority")) - 1);
                    priority = (Priority)Enum.Parse(typeof(Priority), PriorityName);
                }

                var issue = await _issueManager.CreateAsync
                    (name,
                    description,
                    priority,
                    LevelEnum.Level.Undefined,
                    null,
                    project.Id,
                    status.Id,
                    null,
                    DateTime.UtcNow,
                    null,
                    0,
                    UpdateChangesFromTfs.resource.id,
                    UpdateChangesFromTfs.resource.rev,
                    issueParent);

                if (_userInforTfsRepository.Any(x => x.UniqueName.Contains(userUniqueNameCreatedBy)))
                {
                    issue.CreatorId = _userInforTfsRepository.FirstOrDefault(x => x.UniqueName.Contains(userUniqueNameCreatedBy)).UserId;
                }
                else
                {
                    if (_userRepository.Any(x => x.Name.Contains(userNameCreatedBy)))
                    {
                        issue.CreatorId = _userRepository.FirstOrDefault(x => x.Name.Contains(userNameCreatedBy)).Id;
                    }
                    else
                    {
                        issue.CreatorId = _userRepository.FirstOrDefault(x => _userManager.CheckRoleByUserId(x.Id, "admin").Result).Id;
                    }
                }

                var issueCreated = await _issueRepository.InsertAsync(issue);

                if (dictionaryFields.ContainsKey("System.AssignedTo"))
                {
                    string userAssigneeTfs = GetValueDictionaryKey(dictionaryFields, "AssignedTo");
                    string displayNameUserAssignee = userAssigneeTfs;

                    //get unique name
                    int index = userAssigneeTfs.LastIndexOf("_") + 1;
                    userAssigneeTfs = userAssigneeTfs.Remove(0, index).Replace(">", "");
                    //get display name
                    int indexName = displayNameUserAssignee.LastIndexOf("<") - 1;
                    var userNameAssigneeTo = displayNameUserAssignee.Remove(indexName, displayNameUserAssignee.Length - indexName);

                    if (_userInforTfsRepository.Any(x => x.UniqueName.Contains(userAssigneeTfs)))
                    {
                        UserInforTFS.UserInforTfs userInforTfs = GetUserEqualToUserAssigneeTfsAsync(userAssigneeTfs, userNameAssigneeTo);

                        if (!_assigneeRepository.Any(x => x.UserID == userInforTfs.UserId && x.IssueID == issue.Id))
                        {
                            var assignee = _assigneeManager.CreateAsync(issue.Id, userInforTfs.UserId);
                            guids.Add(userInforTfs.UserId);
                            var assigneed = await _assigneeRepository.InsertAsync(assignee);
                            await _notificationsAppService.InsertAsync(assigneed.IssueID, assigneed.UserID,
                                _userRepository.FirstOrDefault(x => x.Id == userInforTfs.UserId).Name + " has been assigneed you into an issue: " + issue.Name);
                            var follow = _followManager.CreateAsync(assigneed.IssueID, assigneed.UserID);
                            await _followRepository.InsertAsync(follow);
                        }
                        else
                        {
                            guids.Add(string.Empty);
                        }
                    }
                }
                else
                {
                    guids.Add(string.Empty);
                }

                if (UpdateChangesFromTfs.resource.commentVersionRef != null)
                {
                    if (_userInforTfsRepository.Any(x => x.UniqueName.Contains(userUniqueNameCreatedBy)))
                    {
                        UserInforTFS.UserInforTfs userInforTfs = GetUserEqualToUserChangeTfsAsync(userUniqueNameCreatedBy, userNameCreatedBy);
                        var comment = _commentManager.Create(issueCreated.Id, userInforTfs.UserId, GetValueDictionaryKey(dictionaryFields, "History"),
                            UpdateChangesFromTfs.resource.commentVersionRef.commentId);
                        Comments.Comment commentCreated = await _commentRepository.InsertAsync(comment);
                    }
                }
                guids.Add(issueCreated.ProjectID.ToString());
                guids.Add(issueCreated.Id.ToString());
                await _hub.Clients.All.SendAsync("ChangesFromTfs", guids);
            }
        }
        public async Task PostCatchDeleteWit(UpdateChangesFromTfs UpdateChangesFromTfs)
        {
            if (_issueRepository.Any(x => x.IdWIT == UpdateChangesFromTfs.resource.id))
            {
                List<string> guids = new List<string>();
                guids.Add(string.Empty);
                var issue = await _issueRepository.GetAsync(x => x.IdWIT == UpdateChangesFromTfs.resource.id);

                var follows = await _followRepository.GetListAsync(x => x.IssueID == issue.Id);
                var assignees = await _assigneeRepository.GetListAsync(x => x.IssueID == issue.Id);
                var comments = await _commentRepository.GetListAsync(x => x.IssueID == issue.Id);
                var attachments = await _attachmentRepository.GetListAsync(x => x.TableID == issue.Id);

                if (follows.Any())
                {
                    await _followRepository.DeleteManyAsync(follows);
                }
                if (assignees.Any())
                {
                    await _assigneeRepository.DeleteManyAsync(assignees);
                }
                if (comments.Any())
                {
                    await _commentRepository.DeleteManyAsync(comments);
                }
                if (attachments.Any())
                {
                    await _attachmentRepository.DeleteManyAsync(attachments);
                }

                await _issueRepository.DeleteAsync(issue);

                guids.Add(issue.ProjectID.ToString());
                guids.Add(issue.Id.ToString());
                await _hub.Clients.All.SendAsync("ChangesFromTfs", guids);
            }
        }

        public async Task<commentsTfs> GetComments(int id, string projectName, string CurrentUserId)
        {
            var commentsTfs = JsonConvert.DeserializeObject<commentsTfs>(await GetHttpClient("/" + projectName + "/_apis/wit/workItems/" + id + "/comments?api-version=6.1-preview.3", CurrentUserId));
            return commentsTfs;
        }

        public async Task<WitState> GetStateAsync(string projectName, string witType, string currentUserId)
        {
            string url = "/" + projectName + "/_apis/wit/workitemtypes/" + witType + "/states?api-version=6.1-preview.1";
            var rs = await GetHttpClient(url, currentUserId);
            return JsonConvert.DeserializeObject<WitState>(rs);

        }

        public async Task UpdateState(Guid id, Guid statusId, string CurrentUserId)
        {
            if (_azureRepository.Any())
            {
                var status = await _statusRepository.GetAsync(statusId);
                var issue = await _issueRepository.GetAsync(id);
                if (issue.IdWIT != 0)
                {
                    var project = await _projectRepository.GetAsync(issue.ProjectID);
                    if (project.ProjectIdTFS != Guid.Empty)
                    {
                        if (issue.IdParent != Guid.Empty)
                        {
                            project.WitType = "Task";
                        }
                        string state = "";
                        List<object> WitFields = new List<object> { };
                        var wit = await GetWIT(issue.IdWIT, project.Name, CurrentUserId);
                        WitState states = await GetStateAsync(project.Name, 
                            wit.fields.FirstOrDefault(x => x.Key.Contains("System.WorkItemType")).Value.ToString(), 
                            CurrentUserId);
                        var anyStateEqualStatus = states.value.FirstOrDefault(x => x.name == status.Name);

                        if (anyStateEqualStatus != null)
                        {
                            state = anyStateEqualStatus.name;
                        }
                        else
                        {
                            if (status.CurrentIndex > states.count)
                            {
                                state = states.value[states.count - 2].name;
                            }
                            else
                            {
                                state = states.value[status.CurrentIndex].name;
                            }
                        }

                        var stateField = new { op = "add", path = "/fields/System.State", value = state };
                        WitFields.Add(stateField);
                        string json = JsonConvert.SerializeObject(WitFields);
                        var url = "/" + project.Name + "/_apis/wit/workitems/" + issue.IdWIT + "?api-version=6.1-preview.3";
                        var responseBody = await MethodHttpClient(url, json, "PATCH", CurrentUserId);
                        var WitUpdated = JsonConvert.DeserializeObject<WIT>(responseBody);
                        await UpdateRev(issue.Id, WitUpdated.rev);
                    }
                }
            }

        }
        public async Task InsertComment(Guid idComment, Guid issueId, string commentContentent, string projectName, string CurrentUserId)
        {
            if (_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                var issue = await _issueRepository.GetAsync(issueId);
                var project = await _projectRepository.GetAsync(x => x.Id == issue.ProjectID);
                if (project.ProjectIdTFS != Guid.Empty)
                {
                    if (issue.IdWIT != 0)
                    {
                        var commentCon = new commentContent() { };
                        commentCon.text = commentContentent;
                        var url = "/" + projectName + "/_apis/wit/workitems/" + issue.IdWIT + "/comments?api-version=6.1-preview.3";
                        var responseBody = await PostAsJsonHttpClient(url, commentCon, CurrentUserId);
                        var CommentCreated = JsonConvert.DeserializeObject<commentCreated>(responseBody);
                        var comment = await _commentRepository.GetAsync(idComment);
                        comment.WitCommentId = CommentCreated.id;
                        await _commentRepository.UpdateAsync(comment);
                    }
                }
            }
        }

        public async Task DeleteComment(Guid IdWIT, string projectName, int idCommentWIT, string CurrentUserId)
        {
            if (_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                var issue = await _issueRepository.GetAsync(IdWIT);
                var project = await _projectRepository.GetAsync(x => x.Id == issue.ProjectID);
                if (project.ProjectIdTFS != Guid.Empty)
                {
                    if (issue.IdWIT != 0)
                    {
                        await DeleteHttpClient("/" + projectName + "/_apis/wit/workItems/" + issue.IdWIT + "/comments/" + idCommentWIT + "?api-version=6.1-preview.3", CurrentUserId);
                        await UpdateRev(issue.Id, issue.Rev + 1);
                    }
                }
            }
        }
        public async Task UpdateComment(Guid id, Guid IdWIT, string commentContentent, string projectName, string CurrentUserId)
        {
            if (_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                var issue = await _issueRepository.GetAsync(IdWIT);
                var project = await _projectRepository.GetAsync(x => x.Id == issue.ProjectID);
                if (project.ProjectIdTFS != Guid.Empty)
                {
                    if (issue.IdWIT != 0)
                    {
                        var comment = await _commentRepository.GetAsync(id);
                        var commentCon = new commentContent() { };
                        commentCon.text = commentContentent;
                        await PatchHttpClient("/" + projectName + "/_apis/wit/workItems/" + issue.IdWIT + "/comments/" + comment.WitCommentId + "?api-version=6.1-preview.3",
                            commentCon, typeof(commentContent), CurrentUserId);
                        await UpdateRev(issue.Id, issue.Rev + 1);
                    }
                }
            }
        }
        public async Task UpdateParentWit(int childWitId, int parentWitId, string projectName, string CurrentUserId)
        {
            List<object> WitFields = new List<object> { };
            value value = new value();

            var Host = GetHost();

            value.url = Host.Host + "/" + Host.Collection + "/" + projectName + "/_apis/wit/workItems/" + parentWitId;

            var obj = new { op = "add", path = "/relations/-", value = value };
            WitFields.Add(obj);

            string json = JsonConvert.SerializeObject(WitFields);

            var url = "/" + projectName + "/_apis/wit/workitems/" + childWitId + "?api-version=6.1-preview.3";
            await MethodHttpClient(url, json, "PATCH", CurrentUserId);
        }
        public async Task UpdateWIT(Guid id, string name, string description, string priority, List<string> Assignees, string CurrentUserId)
        {
            var issue = await _issueRepository.GetAsync(id);
            if (issue.IdWIT != 0)
            {
                var project = await _projectRepository.GetAsync(x => x.Id == issue.ProjectID);
                string Tags = "";

                List<object> WitFields = new List<object>
                {
                    new { op = "add", path = "/fields/System.Title", value = name },
                    new { op = "add", path = "/fields/System.Description", value = description != null ? description : "" },
                    new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = priority }
                };
                if (Assignees.Any())
                {
                    if (_userInforTfsRepository.Any(x => x.UserId == Assignees.FirstOrDefault()))
                    {
                        var user = await _userInforTfsRepository.GetAsync(x => x.UserId == Assignees.FirstOrDefault());
                        var assignedToField = new { op = "add", path = "/fields/System.AssignedTo", value = user.UniqueName };
                        WitFields.Add(assignedToField);
                    }

                    if (Assignees.Count > 1)
                    {
                        for (var i = 1; i < Assignees.Count; i++)
                        {
                            var userFor = _userRepository.FirstOrDefault(x => x.Id == Assignees[i]).Name;
                            Tags = Tags + userFor + ";";
                        }
                        var tagsField = new { op = "add", path = "/fields/System.Tags", value = Tags };
                        WitFields.Add(tagsField);
                    }
                    else
                    {
                        var tagsField = new { op = "add", path = "/fields/System.Tags", value = Tags };
                        WitFields.Add(tagsField);
                    }
                }
                else
                {
                    var assignedToField = new { op = "add", path = "/fields/System.AssignedTo", value = "" };
                }

                string json = JsonConvert.SerializeObject(WitFields);
                var url = "/" + project.Name + "/_apis/wit/workitems/" + issue.IdWIT + "?api-version=6.1-preview.3";
                var responseBody = await MethodHttpClient(url, json, "PATCH", CurrentUserId);
                var WitUpdate = JsonConvert.DeserializeObject<WIT>(responseBody);
                await UpdateRev(issue.Id, WitUpdate.rev);
            }
        }
        public async Task CreateWIT(Guid id, string projectName, string name, string description, string priority, List<string> Assignees, string CurrentUserId, Guid issueParentId)
        {
            var project = await _projectRepository.GetAsync(x => x.Name == projectName);
            string Tags = "";
            var type = issueParentId != Guid.Empty ? "Task" : project.WitType;
            List<object> WitFields = new List<object>
                {
                    new { op = "add", path = "/fields/System.Title", value = name },
                    new { op = "add", path = "/fields/System.Description", value = description != null ? description : "" },
                    new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = priority }
                };
            if (Assignees.Any())
            {
                var ToAssigneeList = Assignees.ToList();

                if (_userInforTfsRepository.Any(x => x.UserId == ToAssigneeList.FirstOrDefault()))
                {
                    var user = await _userInforTfsRepository.GetAsync(x => x.UserId == ToAssigneeList.FirstOrDefault());
                    var assignedToField = new { op = "add", path = "/fields/System.AssignedTo", value = user.UniqueName };
                    WitFields.Add(assignedToField);
                }

                if (Assignees.Count > 1)
                {
                    for (var i = 1; i < ToAssigneeList.Count; i++)
                    {
                        var userFor = _userRepository.FirstOrDefault(x => x.Id == ToAssigneeList[i]).Name;
                        Tags = Tags + userFor + ";";
                    }
                    var tagsField = new { op = "add", path = "/fields/System.Tags", value = Tags };
                    WitFields.Add(tagsField);
                }
            }
            string json = JsonConvert.SerializeObject(WitFields);
            var url = "/" + project.Name + "/_apis/wit/workitems/$" + type + "?api-version=6.1-preview.3";
            var responseBody = await MethodHttpClient(url, json, "POST", CurrentUserId);
            var WitCreated = JsonConvert.DeserializeObject<WIT>(responseBody);
            if (issueParentId != Guid.Empty)
            {
                var issueParent = await _issueRepository.GetAsync(x => x.Id == issueParentId);
                await UpdateParentWit(WitCreated.id, issueParent.IdWIT, projectName, CurrentUserId);
            }
            var issue = await _issueRepository.GetAsync(id);
            issue.IdWIT = WitCreated.id;
            issue.Rev = WitCreated.rev;
            await _issueRepository.UpdateAsync(issue);
            var openStatus = await _statusRepository.GetAsync(x => x.Name == "Open");
            if(issue.StatusID != openStatus.Id)
            {
                await UpdateState(issue.Id, issue.StatusID, CurrentUserId);
            }
        }
        public async Task DeleteWIT(int id, Guid projectId, string CurrentUserId)
        {
            var project = await _projectRepository.GetAsync(projectId);
            await DeleteHttpClient("/" + project.Name + "/_apis/wit/workitems/" + id + "?api-version=6.1-preview.3", CurrentUserId);
        }
        public async Task UpdateAssignee(Guid id, List<string> Assignees, bool IsDefault, string CurrentUserId)
        {
            if (IsDefault == false)
            {
                string Tags = "";
                var issue = await _issueRepository.GetAsync(id);
                if (issue.IdWIT != 0)
                {
                    var project = await _projectRepository.GetAsync(x => x.Id == issue.ProjectID);
                    if (project.ProjectIdTFS != Guid.Empty)
                    {
                        List<object> WitFields = new List<object> { };
                        if (Assignees.Any())
                        {
                            var AssigneeList = _assigneeRepository.Count(x => x.IssueID == issue.Id);
                            if (AssigneeList > Assignees.Count)
                            {
                                for (var i = 0; i < Assignees.Count; i++)
                                {
                                    var userFor = _userRepository.FirstOrDefault(x => x.Id == Assignees[i]).Name;
                                    Tags = Tags + userFor + ";";
                                }
                                var tagsField = new { op = "add", path = "/fields/System.Tags", value = Tags };
                                WitFields.Add(tagsField);
                            }
                            else
                            {
                                if (_userInforTfsRepository.Any(x => x.UserId == Assignees.FirstOrDefault()))
                                {
                                    var user = await _userInforTfsRepository.GetAsync(x => x.UserId == Assignees.FirstOrDefault());
                                    var assignedToField = new { op = "add", path = "/fields/System.AssignedTo", value = user.UniqueName };
                                    WitFields.Add(assignedToField);

                                }

                                if (Assignees.Count > 1)
                                {
                                    for (var i = 1; i < Assignees.Count; i++)
                                    {
                                        var userFor = _userRepository.FirstOrDefault(x => x.Id == Assignees[i]).Name;
                                        Tags = Tags + userFor + ";";
                                    }
                                    var tagsField = new { op = "add", path = "/fields/System.Tags", value = Tags };
                                    WitFields.Add(tagsField);
                                }
                                else if(Assignees.Count == 1)
                                {
                                    var tagsField = new { op = "add", path = "/fields/System.Tags", value = "" };
                                    WitFields.Add(tagsField);
                                }
                            }
                        }
                        else
                        {
                            var tagsField = new { op = "add", path = "/fields/System.Tags", value = Tags };
                            WitFields.Add(tagsField);
                            var assignedToField = new { op = "add", path = "/fields/System.AssignedTo", value = "" };
                            WitFields.Add(assignedToField);
                        }

                        string json = JsonConvert.SerializeObject(WitFields);
                        var url = "/" + project.Name + "/_apis/wit/workitems/" + issue.IdWIT + "?api-version=6.1-preview.3";
                        var responseBody = await MethodHttpClient(url, json, "PATCH", CurrentUserId);
                        var WitUpdated = JsonConvert.DeserializeObject<WIT>(responseBody);
                        await UpdateRev(issue.Id, WitUpdated.rev);
                    }
                }

            }
        }

        public async Task<WIT> GetWIT(int id, string projectName, string currentUserId)
        {
            var responseBody = await GetHttpClient("/" + projectName + "/_apis/wit/workitems/" + id + "?$expand=all&api-version=6.1-preview.3", currentUserId);
            return JsonConvert.DeserializeObject<WIT>(responseBody);
        }

        public async Task<WITsDto> GetWITs(Guid projectId, int take, int skip, string filter, string state, string type)
        {
            var currentUserId = _tmtCurrentUser.GetId();
            var WITsMap = new WITsDto();
            var project = await _projectRepository.GetAsync(x => x.Id == projectId);

            if (filter.IsNullOrWhiteSpace() || filter == "null")
            {
                filter = "";
            }

            if (state.IsNullOrWhiteSpace() || state == "null")
            {
                state = "";
            }

            if (type.IsNullOrWhiteSpace() || type == "null")
            {
                type = "";
            }

            if (project.ProjectIdTFS != Guid.Empty)
            {
                var issues = await _issueRepository.GetListAsync(x => x.ProjectID == projectId && x.Name.Contains(filter) && x.IdWIT != 0);
                if(issues.Count > 0)
                {
                    string ids = "";

                    for (var i = 0; i < issues.Count; i++)
                    {
                        if (i == issues.Count - 1)
                        {
                            ids += issues[i].IdWIT;
                        }
                        else
                        {
                            ids += issues[i].IdWIT + ",";
                        }
                    }

                    var responseBody = await GetHttpClient("/" + project.Name + "/_apis/wit/workitems?ids=" + ids + "&api-version=6.1-preview.3", currentUserId);
                    var rs = JsonConvert.DeserializeObject<WITs>(responseBody);

                    WITsMap.value = new List<WITDto>();
                    WITsMap.state = new List<string>();
                    WITsMap.type = new List<string>();

                    var result = rs.value.Where(x => GetValueDictionaryKey(x.fields, "System.State").Contains(state)
                    && GetValueDictionaryKey(x.fields, "System.WorkItemType").Contains(type))
                        .Skip(skip)
                        .Take(take)
                        .ToList<WIT>();

                    foreach (var wit in result)
                    {
                        var witMap = new WITDto();
                        var dictionary = wit.fields;

                        witMap.name = GetValueDictionaryKey(dictionary, "System.Title");
                        witMap.state = GetValueDictionaryKey(dictionary, "System.State");
                        witMap.reason = GetValueDictionaryKey(dictionary, "System.Reason");
                        witMap.CreatedDate = GetValueDictionaryKey(dictionary, "System.CreatedDate");
                        witMap.type = GetValueDictionaryKey(dictionary, "System.WorkItemType");
                        witMap.AreaPath = GetValueDictionaryKey(dictionary, "System.AreaPath");
                        witMap.TeamProject = GetValueDictionaryKey(dictionary, "System.TeamProject");
                        witMap.IterationPath = GetValueDictionaryKey(dictionary, "System.IterationPath");

                        WITsMap.value.Add(witMap);

                        if (!WITsMap.state.Any(x => x == witMap.state))
                        {
                            WITsMap.state.Add(witMap.state);
                        }

                        if (!WITsMap.type.Any(x => x == witMap.type))
                        {
                            WITsMap.type.Add(witMap.type);
                        }
                    }

                    WITsMap.count = rs.count;

                }
                
            }

            return WITsMap;
        }

        public async Task<VssConnection> GetConnectionAsync(Guid projectId)
        {
            var Host = _azureRepository.SingleOrDefault();
            var project = await _projectRepository.GetAsync(projectId);
            var Pat = await _userInforTfsRepository.GetAsync(x => x.UserId == project.CreatorId);
            var creds = new VssBasicCredential(string.Empty, Pat.PAT);

            VssConnection connection = new VssConnection(new Uri(Host.Host + "/" + Host.Collection), creds);

            return connection;
        }
        public async Task<TeamProject> GetProjectAsync(Guid projectId)
        {
            var connection = await GetConnectionAsync(projectId);
            var projectClient = connection.GetClient<ProjectHttpClient>();
            var project = await _projectRepository.GetAsync(projectId);

            var rs = await projectClient.GetProject(project.ProjectIdTFS.ToString());

            return rs;
        }
        public async Task CreateMember(Guid projectId, string accountName)
        {
            if (_userInforTfsRepository.Any(x => x.UniqueName.Contains(accountName)))
            {
                var project = await GetProjectAsync(projectId);
                var connection = await GetConnectionAsync(projectId);
                var client = connection.GetClient<IdentityHttpClient>();
                if (accountName.Contains("SYSTMT\'")) accountName = accountName.Replace("SYSTMT\'", "");

                var identities = await client.ReadIdentitiesAsync(Microsoft.VisualStudio.Services.Identity.IdentitySearchFilter.AccountName, accountName);

                if (!identities.Any() || identities.Count > 1)
                {
                    throw new UserFriendlyException("User not found or could not get an exact match based on account Name");
                }

                var userIdentity = identities.Single();
                var groupIdentity = await client.ReadIdentityAsync(project.DefaultTeam.Id);

                await client.AddMemberToGroupAsync(
                    groupIdentity.Descriptor,
                    userIdentity.Id
                );
            }
        }
        public async Task<List<WorkItem>> GetWitsByWiql(string url, string pAT, string project, string title, string types, string states, ConditionType conditionType)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            var creds = new VssBasicCredential(string.Empty, pAT);
            var witType = "";
            var witSate = "";

            if (types != null && types.Replace(" ", "") != "")
            {
                var typesSplit = types.Split(",");

                switch (typesSplit.Length)
                {
                    case 0: 
                        break;
                    case 1:
                        witType = types != null ? " AND [Work Item Type] = '" + typesSplit[0] + "'" : "";
                        break;
                    case > 1:
                        for(var i = 0; i < typesSplit.Length; i++)
                        {
                            if(i == 0)
                            {
                                witType = " AND ([Work Item Type] = '" + typesSplit[0] + "'";
                            }
                            else if(i == typesSplit.Length - 1)
                            {
                                witType = witType + " OR [Work Item Type] = '" + typesSplit[i] + "')";
                            }
                            else
                            {
                                witType = witType + " OR [Work Item Type] = '" + typesSplit[i] + "'";
                            }
                        }
                        break;
                }
            }

            if (states != null && states.Replace(" ", "") != "")
            {
                var statesSplit = states.Split(",");

                switch (statesSplit.Length)
                {
                    case 0:
                        break;
                    case 1:
                        witSate = states != null ? " AND [State] = '" + statesSplit[0] + "'" : "";
                        break;
                    case > 1:
                        for (var i = 0; i < statesSplit.Length; i++)
                        {
                            if (i == 0)
                            {
                                witSate = " AND ([State] = '" + statesSplit[0] + "'";
                            }
                            else if(i == statesSplit.Length - 1)
                            {
                                witSate = witSate + " OR [State] = '" + statesSplit[i] + "')";
                            }
                            else 
                            {
                                witSate = witSate + " OR [State] = '" + statesSplit[i] + "'";
                            }
                        }
                        break;
                }
            }

            var witProject = project != null ? " AND [Area Path] = '" + project + "'" : "";
            var witTitle = title != null ? " AND [Title] Contains '" + title + "'" : "";

            WorkItemTrackingHttpClient witClient = new WorkItemTrackingHttpClient(new Uri(url), creds);
            Wiql query;

            if (conditionType == 0)
            {
                query = new Wiql() { Query = "SELECT [Id] FROM workitems WHERE [Assigned To] = @Me" + witProject + witType + witSate + witTitle };
            }
            else
            {
                query = new Wiql() { Query = "SELECT [Id] FROM workitems WHERE [Id] IN(@follows)" + witProject + witType + witSate + witTitle };
            }
            
            WorkItemQueryResult queryResults = witClient.QueryByWiqlAsync(query).Result;

            if (queryResults.WorkItems.Count() != 0)
            {
                var ids = queryResults.WorkItems.Select(x => x.Id);

                workItems = await witClient.GetWorkItemsAsync(project != null ? project : "", ids);
            }

            return workItems;
        }

        public async Task<List<WorkItem>> SyncProjectFromTfsAsync(string url, Guid projectId)
        {
            var currentUserId = _tmtCurrentUser.GetId();
            var project = url.Split("/")[4];
            var urlProject = url.Replace(project, "");
            if(!_userInforTfsRepository.Any(x => x.UserId == currentUserId))
            {
                throw new UserFriendlyException("Cannot get Access Token from DB. Please update your Token!");
            }
            var pAT = _userInforTfsRepository.FirstOrDefault(x => x.UserId == currentUserId).PAT;
            var creds = new VssBasicCredential(string.Empty, pAT);
            var witProject = "[Area Path] = '" + project + "'";
            List<WorkItem> workItems = new List<WorkItem>();

            WorkItemTrackingHttpClient witClient = new WorkItemTrackingHttpClient(new Uri(urlProject), creds);
            TeamHttpClient teamHttpClient = new TeamHttpClient(new Uri(urlProject), creds);
            var teams = await teamHttpClient.GetTeamsAsync(project);

            foreach(WebApiTeam webApiTeam in teams)
            {
                var members = await teamHttpClient.GetTeamMembersWithExtendedPropertiesAsync(webApiTeam.ProjectId.ToString(), webApiTeam.Id.ToString());
                foreach(TeamMember teamMember in members)
                {
                    if (!_userInforTfsRepository.Any(x => x.UniqueName.Replace("\\", "") == teamMember.Identity.UniqueName.Replace("\\", "")))
                    {
                        throw new UserFriendlyException("User not in T-BugTracking or not update unique Name: " + teamMember.Identity.DisplayName);
                    }

                    var userTeam = GetUserEqualToUserChangeTfsAsync(teamMember.Identity.UniqueName.Replace("\\",""), teamMember.Identity.DisplayName);

                    if(!_memberRepository.Any(x => x.ProjectID == projectId && x.UserID == userTeam.UserId))
                    {
                        var team = await _memberManager.CreateAsync(projectId, userTeam.UserId);
                        await _memberRepository.InsertAsync(team);
                    }
                }
            }

            var projectTfs = await _projectRepository.GetAsync(projectId);
            projectTfs.ProjectIdTFS = teams.FirstOrDefault().ProjectId;
            await _projectRepository.UpdateAsync(projectTfs);

            Wiql query = new Wiql() { Query = "SELECT [Id] FROM workitems WHERE " + witProject };

            WorkItemQueryResult queryResults = witClient.QueryByWiqlAsync(query).Result;

            if (queryResults.WorkItems.Count() != 0)
            {
                List<WITCreatedTemp> wITCreatedTemps = new List<WITCreatedTemp>();
                var ids = queryResults.WorkItems.Select(x => x.Id);

                workItems = await witClient.GetWorkItemsAsync(project, ids, null , null, WorkItemExpand.All);
                workItems = workItems.OrderBy(x => x.Fields.ContainsKey("System.Parent")).ToList();

                foreach(WorkItem workItem in workItems)
                {
                    IDictionary<string,object> dictionaryKey = workItem.Fields;

                    var title = GetValueIDictionaryKey(dictionaryKey, "Title").ToString();
                    var state = GetValueIDictionaryKey(dictionaryKey, "State").ToString();
                    var createdBy = GetValueIDictionaryKey(dictionaryKey, "CreatedBy");
                    var des = GetValueIDictionaryKey(dictionaryKey, "Description") != null ? GetValueIDictionaryKey(dictionaryKey, "Description").ToString() : "";
                    var commentCount = int.Parse(GetValueIDictionaryKey(dictionaryKey, "CommentCount").ToString());

                    int idParentWit = 0;
                    if (dictionaryKey.ContainsKey("System.Parent"))
                    {
                        idParentWit = int.Parse(GetValueIDictionaryKey(dictionaryKey, "Parent").ToString());
                    }

                    var assigneeResult = GetValueIDictionaryKey(dictionaryKey, "AssignedTo");

                    string displayName = "";
                    string uniqueName = "";
                    var assignedTo = new string[0];

                    if (assigneeResult != null)
                    {
                        System.Reflection.PropertyInfo piDisplayName = assigneeResult.GetType().GetProperty("DisplayName");
                        displayName = (String)(piDisplayName.GetValue(assigneeResult, null));

                        System.Reflection.PropertyInfo piUniqueName = assigneeResult.GetType().GetProperty("UniqueName");
                        uniqueName = (String)(piUniqueName.GetValue(assigneeResult, null));

                        assignedTo = new string[1] {
                        GetUserEqualToUserAssigneeTfsAsync(uniqueName, displayName).UserId };
                    }


                    Guid statusId = Guid.Empty;

                    if(_statusRepository.Any(x => x.Name == state))
                    {
                      statusId = _statusRepository.FirstOrDefault(x => x.Name == state).Id;
                    }
                    else
                    {
                        if(state == "New" || state == "To Do")
                        {
                            statusId = _statusRepository.FirstOrDefault(x => x.Name == "Open").Id;
                        }
                        else
                        {
                            statusId = _statusRepository.FirstOrDefault(x => x.Name == "In Progress").Id;
                        }
                    }



                    CreateUpdateIssuesDto createUpdateIssuesDto = new CreateUpdateIssuesDto();
                    var attachments = new List<CreateDetailAttachmentDto>();

                    string PriorityName = Enum.GetName(typeof(Priority), int.Parse(GetValueIDictionaryKey(dictionaryKey, "Priority").ToString()) - 1);
                    var priority = (Priority)Enum.Parse(typeof(Priority), PriorityName);

                    createUpdateIssuesDto.Name = title;
                    createUpdateIssuesDto.Description = des;
                    createUpdateIssuesDto.StatusID = statusId;
                    createUpdateIssuesDto.ProjectID = projectId;
                    createUpdateIssuesDto.IssueLevel = LevelEnum.Level.Undefined;
                    createUpdateIssuesDto.NotifyMail = new string[0];
                    createUpdateIssuesDto.Attachments = attachments;
                    createUpdateIssuesDto.Priority = priority;
                    createUpdateIssuesDto.Assignees = assignedTo;
                    createUpdateIssuesDto.IdParent = idParentWit != 0 ? wITCreatedTemps.FirstOrDefault(x => x.witId == idParentWit).issueId : Guid.Empty;
                    createUpdateIssuesDto.IdWit = (int)workItem.Id;
                    createUpdateIssuesDto.StartDate = DateTime.Now;

                    var  issueCreated = await _issueAppService.CreateIssueAttachment(createUpdateIssuesDto);

                    var WitCreated = new WITCreatedTemp();
                    WitCreated.witId = (int)workItem.Id;
                    WitCreated.issueId = issueCreated.Id;

                    wITCreatedTemps.Add(WitCreated);

                    if (commentCount > 0)
                    {
                        var comments = await GetComments((int)workItem.Id, project, currentUserId);
                        foreach(var comment in comments.comments)
                        {
                            var displayNameCm = GetValueDictionaryKey(comment.createdBy, "displayName");
                            var uniqueNameCm = GetValueDictionaryKey(comment.createdBy, "uniqueName");
                            var userCm = GetUserEqualToUserChangeTfsAsync(uniqueNameCm, displayNameCm).UserId;

                            var commentCreated = _commentManager.Create(issueCreated.Id,
                                userCm,
                                comment.text, 
                                comment.id);

                            await _commentRepository.InsertAsync(commentCreated);
                        }
                    }
                }
            }

            return workItems;
        }

        private async Task<string> GetWitIds(string url, string PAT, string project, string title, string type, string state)
        {
            using (HttpClient client = new HttpClient())
            {
                AuthorizeWithPat(client, PAT);

                var witType = type != null ? " AND [Work Item Type] = '" + type + "'" : "";
                var witSate = state != null ? " AND [State] = '" + state + "'" : "";
                var witProject = " AND [Area Path] = '" + project + "'";
                var witTitle = title != null ? " AND [Title] Contains Words '" + title + "'" : "";

                var wiqlQuery = new WiqlQuery() { };
                wiqlQuery.query = "SELECT [Id] FROM workitems WHERE [Assigned To] = @Me" + witProject + witType + witSate + witTitle;

                using (HttpResponseMessage response = await client.PostAsJsonAsync(url + "/_apis/wit/wiql?api-version=6.0", wiqlQuery))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new UserFriendlyException(await response.Content.ReadAsStringAsync());
                    }
                    var queryResult = JsonConvert.DeserializeObject<workItemsQuery>(responseBody);

                    string ids = "";

                    for (var i = 0; i < queryResult.workItems.Count; i++)
                    {
                        if (i == queryResult.workItems.Count - 1)
                        {
                            ids += queryResult.workItems[i].id;
                        }
                        else
                        {
                            ids += queryResult.workItems[i].id + ",";
                        }
                    }
                    return ids;
                }
            }
        }
        

    }
}
