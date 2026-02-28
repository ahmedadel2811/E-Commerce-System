using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    // سجل التدقيق
    public class AuditLog : BaseEntity
    {
        public string EntityName { get; set; } // اسم الكيان المتعلق
        public string ActionType { get; set; } // Create, Update, Delete, Login, etc. نوع الإجراء
        public string UserId { get; set; } // يمكن أن يكون السجل متعلقًا بمستخدم معين
        public string? OldValues { get; set; } // القيم القديمة
        public string? NewValues { get; set; } // القيم الجديدة
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
