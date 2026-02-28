using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    public class Notification : BaseEntity
    {

        public string Title { get; set; } // عنوان الإشعار
        public string Message { get; set; } // محتوى الإشعار
        public string Type { get; set; } // Order, System, Promotion نوع الإشعار : طلب ، نظام، ترويج
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // كب إشعار يخص مستخدم واحد
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
