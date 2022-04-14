using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;

namespace BugTracking.Conversation
{
   public class Conversation : TMTAuditedAggregateRoot<Guid>
    {
        public string ConversationId { get; set;}
        public string idProject { get; set; }
        private Conversation()
        {
            /* This constructor is for deserialization / ORM purpose */
        }
        internal Conversation(
                Guid id,
                [NotNull] string conversationID,
                [NotNull] string idproject
            )
            : base(id)
        {
            ConversationId = conversationID;
            idProject = idproject;
        }
    }
}
