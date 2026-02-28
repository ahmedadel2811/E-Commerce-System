using ECommerce.Core.DTOs.Notification;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        
        [HttpGet("GetNotificationById")]
        [Authorize]
        public async Task<ActionResult<Response<NotificationDto>>> GetNotification(int id)
        {
            var result = await _notificationService.GetNotificationByIdAsync(id);

            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("GetUserNotifications")]
        [Authorize]
        public async Task<ActionResult<Response<List<NotificationDto>>>> GetUserNotifications(string userId)
        {
            var result = await _notificationService.GetUserNotificationsAsync(userId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("GetUnreadNotifications")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<List<NotificationDto>>>> GetUnreadNotifications(string userId)
        {
            var result = await _notificationService.GetUnreadNotificationsAsync(userId);

            if (!result.Succeeded)
            {
                return NotFound(result);
            }


            return Ok(result);
        }

        [HttpGet("GetUnreadCount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<int>>> GetUnreadCount(string userId)
        {
            var result = await _notificationService.GetUnreadCountAsync(userId);

            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost ("CreateNotification") ]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<NotificationDto>>> CreateNotification(CreateNotificationDto dto)
        {
            var result = await _notificationService.CreateNotificationAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("MarkAsRead")]
        [Authorize]
        public async Task<ActionResult<Response<bool>>> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);

            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPut("MarkAllAsRead")]
        [Authorize]

        public async Task<ActionResult<Response<bool>>> MarkAllAsRead(string userId)
        {
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("Delete")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Response<bool>>> DeleteNotification(int id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            return Ok(result);
        }
    }
}