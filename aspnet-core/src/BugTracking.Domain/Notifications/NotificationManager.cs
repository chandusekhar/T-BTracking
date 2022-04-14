using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.Notifications
{
    public class NotificationManager : DomainService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationManager(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
            //push change
        }

        public async Task<Notification> CreateAsync(
            Guid issueID,
            string userID,
            string message,
            bool isRead
            )
        {
            var query = await _notificationRepository.GetListAsync(x => x.IssueID == issueID && x.UserID == userID);
            return new Notification(
                GuidGenerator.Create(),
                issueID,
                userID,
                message,
                isRead
            );
        }
    }
}
