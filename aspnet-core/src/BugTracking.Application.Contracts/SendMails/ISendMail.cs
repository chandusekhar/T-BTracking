using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.SendMails
{
    public interface ISendMail : IApplicationService
    {
        Task QueueAbpMailAsync(string[] users, string subject, string body);
        Task AutoSendMailAsync();
        Task SendMailInviteAsync(string mail,Guid projectId);
        Task SendMailResponse(string Name, string Email, Guid projectId);
    }
}
