using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {

        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
      
        Task<int> GetUnreadCountAsync(string userId);
    }
}
