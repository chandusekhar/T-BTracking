using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.Conversation
{
    public interface IConversationService : IApplicationService
    {
        Task CreateConversation(CreateConversation input);
    }
}
