using BugTracking.Assignees;
using BugTracking.Issues;
using BugTracking.Projects;
using BugTracking.SendMails;
using BugTracking.Statuss;
using BugTracking.Users;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.SendMail
{
    [Authorize("BugTracking.Users")]
    [RemoteService(IsEnabled = true)]
    public class SendNotifyMailAppService : ApplicationService, ISendMail
    {
        //private readonly ISettingEncryptionService _settingEncryptionService;
        //private readonly IStringEncryptionService _stringEncryptionService;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly UserManager _userManager;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Project, Guid> _projectRepository;
        public SendNotifyMailAppService(
          IRepository<AppUser, string> userRepository,
          IIssueRepository issueRepository,
          IStatusRepository statusRepository,
          IAssigneeRepository assigneeRepository,
          IBackgroundJobClient backgroundJobClient,
          UserManager userManager,
          ITMTCurrentUser tmtCurrentUser,
          IConfiguration configuration,
          IRepository<Project, Guid> projectRepository)
        {
            _userRepository = userRepository;
            _issueRepository = issueRepository;
            _statusRepository = statusRepository;
            _assigneeRepository = assigneeRepository;
            _backgroundJobClient = backgroundJobClient;
            _userManager = userManager;
            _tmtCurrentUser = tmtCurrentUser;
            _configuration = configuration;
            _projectRepository = projectRepository;
        }
        private string Logo = "https://mail.tmtco.asia/custom/webmail/img/loginlogo_2edd6.PNG";
        [AllowAnonymous]
        public async Task AutoSendMailAsync()
        {
            var Today = DateTime.UtcNow;
            if (_issueRepository.Any(x => x.DueDate == Today || x.DueDate < Today))
            {
                var users = await _userRepository.GetListAsync();
                StringBuilder sb = new StringBuilder();
                var Closed = await _statusRepository.FindByNameAsync("Closed");
                foreach (AppUser user in users)
                {
                    bool IsAdmin = await _userManager.CheckRoleByUserId(user.Id, "admin") ? true : false;
                    if (IsAdmin)
                    {
                        sb.Clear();
                        sb.Append("<img src=" + Logo + "  />");
                        sb.Append("<br/><b>This is Daily Report mail:</b>");
                        var queryDueDateToday = await _issueRepository.GetListAsync(x => x.StatusID != Closed.Id
                        && x.DueDate == Today);
                        var queryOverDueDate = await _issueRepository.GetListAsync(x => x.StatusID != Closed.Id
                        && x.DueDate < Today);
                        if (queryDueDateToday.Any())
                        {

                            sb.Append("<b> DueDate Today: </b> " + queryDueDateToday.Count + " issues:<br/>");
                            for (var i = 0; i < queryDueDateToday.Count; i++)
                            {
                                sb.AppendFormat("<br/><b>{0}</b> - DueDate: {1}",
                                queryDueDateToday[i].Name, queryDueDateToday[i].DueDate.Value.ToString("dd/MM/yyyy"));
                                GetAssignees(queryDueDateToday[i].Id, sb);

                            }

                        }
                        if (queryOverDueDate.Any())
                        {
                            sb.Append("<br/><b> OverDueDate: </b> " + queryOverDueDate.Count + " issues:<br/>");
                            for (var i = 0; i < queryOverDueDate.Count; i++)
                            {
                                sb.AppendFormat("<br/><b>{0}</b> - DueDate: {1}",
                                queryOverDueDate[i].Name, queryOverDueDate[i].DueDate.Value.ToString("dd/MM/yyyy"));
                                GetAssignees(queryOverDueDate[i].Id, sb);
                            }
                        }
                    }

                    else
                    {
                        if (_assigneeRepository.Any(x => x.UserID == user.Id))
                        {
                            sb.Clear();
                            sb.Append("<img src=" + Logo + "  />");
                            sb.Append("<br/><b>This is Daily Report mail:</b><br/>");
                            var queryDueDateToday = GetIssuesAssignee(user.Id).Where(x => x.DueDate == Today).ToList();
                            var queryOverDueDate = GetIssuesAssignee(user.Id).Where(x => x.DueDate < Today).ToList();
                            if (queryDueDateToday.Any())
                            {
                                sb.Append("<b> DueDate Today: </b> " + queryDueDateToday.Count + " issues:<br/>");

                                for (var i = 0; i < queryDueDateToday.Count; i++)
                                {
                                    sb.AppendFormat("<br/><b>{0}</b> - DueDate: {1}",
                                    queryDueDateToday[i].Name, queryDueDateToday[i].DueDate.Value.ToString("dd/MM/yyyy"));
                                    GetAssignees(queryDueDateToday[i].Id, sb);
                                }
                            }
                            if (queryOverDueDate.Any())
                            {
                                sb.Append("<b> OverDueDate Today: </b> " + queryOverDueDate.Count + " issues:<br/>");
                                for (var i = 0; i < queryOverDueDate.Count; i++)
                                {
                                    sb.AppendFormat("<br /><b>{0}</b> - DueDate: {1}",
                                    queryOverDueDate[i].Name, queryOverDueDate[i].DueDate.Value.ToString("dd/MM/yyyy"));
                                    GetAssignees(queryOverDueDate[i].Id, sb);
                                }
                            }
                        }
                    }
                    if (_assigneeRepository.Any(x => x.UserID == user.Id) || IsAdmin)
                    {
                        _backgroundJobClient.Enqueue<EmailSending>(x => x.ExecuteAsync(new EmailSendingArgs
                        {
                            EmailAddress = user.Email,
                            Subject = "BugTracking Daily Project Report (" + Today.Day + "/" + Today.Month + "/" + Today.Year + ")",
                            Body = sb.ToString()
                        }));
                    }
                }
            }
        }
        private void GetAssignees(Guid Id, StringBuilder sb)
        {
            var assignees = (from issue in _issueRepository
                             join assign in _assigneeRepository on issue.Id equals assign.IssueID
                             join userJ in _userRepository on assign.UserID equals userJ.Id
                             where issue.Id == Id
                             select new AssigneeDto
                             {
                                 UserName = userJ.Name
                             }).ToList();

            for (var j = 0; j < assignees.Count; j++)
            {
                sb.AppendFormat(" - {0} ",
                assignees[j].UserName);
            }
        }
        private List<IssuesDto> GetIssuesAssignee(string Id)
        {
            var Today = DateTime.UtcNow;
            var issues = (from assignee in _assigneeRepository
                          join issue in _issueRepository on assignee.IssueID equals issue.Id
                          join userJ in _userRepository on assignee.UserID equals userJ.Id
                          join status in _statusRepository on issue.StatusID equals status.Id
                          where status.Name != "Closed" &&
                          assignee.UserID == Id
                          select new IssuesDto
                          {
                              Name = issue.Name,
                              DueDate = issue.DueDate,
                              StatusName = status.Name
                          });
            return issues.ToList();
        }
        public async Task QueueAbpMailAsync(string[] users, string subject, string body)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string ID in users)
            {
                sb.Clear();
                var user = await _userRepository.FindAsync(x => x.Id == ID);
                if (user != null)
                {
                    _backgroundJobClient.Enqueue<EmailSending>(x => x.ExecuteAsync(new EmailSendingArgs
                    {
                        EmailAddress = user.Email,
                        Subject = subject,
                        Body = sb.Append("<img src=" + Logo + "  />") + "<br/>" + body
                    }));
                }
            }
        }
        [AllowAnonymous]
        public async Task SendMailResponse(string Name, string Email, Guid projectId)
        {
            var Today = DateTime.UtcNow;
            var project =await _projectRepository.GetAsync(projectId);
            var projectCreator =await _userRepository.GetAsync(x => x.Id == project.CreatorId);
            if (!IsValidEmail(Email) || !IsValidEmail(projectCreator.Email))
            {
                throw new UserFriendlyException("Check your Email Adress input!");
            }
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            _backgroundJobClient.Enqueue<EmailSending>(x => x.ExecuteAsync(new EmailSendingArgs
            {
                EmailAddress = projectCreator.Email,
                Subject = "BugTracking - Response Mail Invite Into Project (" + Today.Day + "/" + Today.Month + "/" + Today.Year + ")",
                Body = sb.Append("<img src=" + Logo + "  />") + "<br/>" +
                "Hi " + projectCreator.Email + ", " + Name + "(" + Email + ")" + " just joined the " + project.Name + " of BugTracking."
            }));
        }
        [AllowAnonymous]
        public async Task SendMailInviteAsync(string Email, Guid projectId)
        {
            if (!IsValidEmail(Email))
            {
                throw new UserFriendlyException("Check your Email Adress input!");
            }
            var Today = DateTime.UtcNow;
            var CurrentUser = await _userRepository.GetAsync(_tmtCurrentUser.GetId());
            var project = await _projectRepository.GetAsync(projectId);
            string Url= "https://t-btracking.tpos.dev";
#if DEBUG
            Url = "http://localhost:4200";
#endif

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            _backgroundJobClient.Enqueue<EmailSending>(x => x.ExecuteAsync(new EmailSendingArgs
            {
                EmailAddress = Email,
                Subject = "BugTracking - Invite Into Project (" + Today.Day + "/" + Today.Month + "/" + Today.Year + ")",
                Body = sb.Append("<img src=" + Logo + "  />") + "<br/>" +
                "Hi " + Email + ", " + CurrentUser.Name + " invite you into a project of BugTracking ("+project.Name+") <br/>"
                + "To join us, please click the link below. <br/>"
                +
                "<a href='" + Url + "/#/sign-in/" + projectId + "/" + Email + "'>" +
                "BugTracking Send Mail Invite.</a>"
            }));
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
