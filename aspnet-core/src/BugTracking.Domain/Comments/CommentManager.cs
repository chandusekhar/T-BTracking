using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.Comments
{
    public class CommentManager : DomainService
    {
        public CommentManager()
        {
            //push change
        }

        public Comment Create(
            Guid issueID,
            string userID,
            string content,
            int witCommentId
            )
        {
            //var query = await _commentRepository.FindAsync(x => x.IssueID == issueID && x.UserID == userID);
            return new Comment(
                GuidGenerator.Create(),
                issueID,
                userID,
                content,
                witCommentId
            );
        }
    }
}
