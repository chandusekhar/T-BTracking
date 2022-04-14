using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Comments
{
    public class Comment : TMTAuditedAggregateRoot<Guid>, ISoftDelete
    {
        public string Content { get; set; }
        public Guid IssueID { get; set; }
        public string UserID { get; set; }
        [DisableAuditing]
        public override ExtraPropertyDictionary ExtraProperties { get => base.ExtraProperties; protected set => base.ExtraProperties = value; }
        public bool IsDeleted { get; set; }
        [DisableAuditing]
        public int WitCommentId { get; set; }
        private Comment()
        {

        }
        internal Comment(
            Guid id,
            [NotNull] Guid issueID,
            [NotNull] string userID,
            string content,
            int witCommentId)
            : base(id)
        {
            IssueID = issueID;
            UserID = userID;
            Content = content;
            WitCommentId = witCommentId;
        }
    }
}
