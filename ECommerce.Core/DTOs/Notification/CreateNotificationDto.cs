namespace ECommerce.Core.DTOs.Notification
{
    public class CreateNotificationDto
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
}
