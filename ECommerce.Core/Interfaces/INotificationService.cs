using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.Notification;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface INotificationService
    {
        Task<Response<NotificationDto>> GetNotificationByIdAsync(int id);
        Task<Response<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(string userId);
        Task<Response<IEnumerable<NotificationDto>>> GetUnreadNotificationsAsync(string userId);
        Task<Response<int>> GetUnreadCountAsync(string userId);
        Task<Response<NotificationDto>> CreateNotificationAsync(CreateNotificationDto dto);
        Task<Response<bool>> MarkAsReadAsync(int id);
        Task<Response<bool>> MarkAllAsReadAsync(string userId);
        Task<Response<bool>> DeleteNotificationAsync(int id);
    }
}
