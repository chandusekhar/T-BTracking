using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.Notifications
{
    public interface INotificationService : IApplicationService
    {
        Task<NotificationDto> GetByIdAsync(Guid id);
    }
}
