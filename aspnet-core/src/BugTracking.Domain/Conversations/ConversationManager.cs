using BugTracking.Categories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.Conversation
{
   public class ConversationManager : DomainService
    {
        private readonly IConversationRepository _conversationRepository;

        public ConversationManager(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }
        public Conversation CreateAsync(
            [NotNull] string conversationID,
            [NotNull] string idproject

            )
        {
            return new Conversation(
                GuidGenerator.Create(),
                conversationID,
                idproject
            );
        }
    }
}
