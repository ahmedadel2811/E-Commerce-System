using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    // وكيل التوصيل
    public class DeliveryAgent : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string VehicleNumber { get; set; } // رقم المركبة
        public string CurrentStatus { get; set; } // Available / Busy / Offline حالة الوكيل : متاح، مشغول، غير متاح

        // Shipment relation: agent can deliver many shipments, each shipment can include many order items (=> many products)
        // يمكن للوكيل توصيل العديد من الشحنات
        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}