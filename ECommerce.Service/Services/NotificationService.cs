using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.Notification;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Response<NotificationDto>> GetNotificationByIdAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return Response<NotificationDto>.Fail("الإشعار غير موجود");

            var dto = MapToDto(notification);
            return Response<NotificationDto>.Success(dto, "تم جلب الإشعار بنجاح");
        }

        public async Task<Response<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);

            if(notifications?.Any() != true)
            {
                return Response<IEnumerable<NotificationDto>>.Fail("اليوزر ليس له اشعاارت");
            }
            var dtos = notifications.Select(MapToDto).ToList();
            return Response<IEnumerable<NotificationDto>>.Success(dtos, "تم جلب الإشعارات بنجاح");
        }

        public async Task<Response<IEnumerable<NotificationDto>>> GetUnreadNotificationsAsync(string userId)
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);

            if (notifications?.Any() != true)
            {
                return Response<IEnumerable<NotificationDto>>.Fail("جميع الاشعارات مقروءه ");

            }
            var dtos = notifications.Select(MapToDto).ToList();
            return Response<IEnumerable<NotificationDto>>.Success(dtos, "تم جلب الإشعارات غير المقروءة بنجاح");
        }

        public async Task<Response<int>> GetUnreadCountAsync(string userId)
        {
            var count = await _notificationRepository.GetUnreadCountAsync(userId);
            if (count == 0)
            {
                return Response<int>.Fail("جميع الاشعارات لهذا المستخدم مقروءه");
            }
            return Response<int>.Success(count, "تم جلب عدد الإشعارات غير المقروءة");
        }

        public async Task<Response<NotificationDto>> CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                IsRead = false,
                CreatedAt = System.DateTime.UtcNow
            };

            var result = await _notificationRepository.AddAsync(notification);
            var resultDto = MapToDto(result);
            return Response<NotificationDto>.Success(resultDto, "تم إنشاء الإشعار بنجاح");
        }

        public async Task<Response<bool>> MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return Response<bool>.Fail("الإشعار غير موجود");

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
            return Response<bool>.Success(true, " تم تغير الاشعار كمقروء");
        }

        public async Task<Response<bool>> MarkAllAsReadAsync(string userId)
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);

            if(notifications?.Any() != true)
            {
                return Response<bool>.Fail( "جميع الاشعارات مقروءه");

            }
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
            }
            return Response<bool>.Success(true, "تم جعل جميع الاشعارات مقروءه");
        }

        public async Task<Response<bool>> DeleteNotificationAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return Response<bool>.Fail("الإشعار غير موجود");

            await _notificationRepository.DeleteAsync(notification);
            return Response<bool>.Success(true, "تم حذف الإشعار بنجاح");
        }

        private NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                UserId = notification.UserId
            };
        }
    }
}