using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    //الشحنة ترتبط بـ
    //DeliveryAgent
    //وتحتوي على مجموعة
    //OrderItems 
    //يبقى الدليفري يقدر يوصل أكثر من منتج
    public class Shipment : BaseEntity
    {


        public string CourierName { get; set; } // اسم شركة الشحن
        public string TrackingNumber { get; set; } // رقم التتبع
        public DateTime? ShippedDate { get; set; } // تاريخ الشحن
        public DateTime? EstimatedDelivery { get; set; } // تاريخ التسليم المتوقع
        public DateTime? DeliveredDate { get; set; } // تاريخ التسليم الفعلي

        //  shipment قد تكون مرتبطة بطلب واحد أو متعددة عبر  OrderItem references
        public int? OrderId { get; set; }   
        public Order Order { get; set; }

        //  الشحنة يمكن أن يتم شحنها بواسطة وكيل توصيل واحد
        public int? DeliveryAgentId { get; set; }
        public DeliveryAgent DeliveryAgent { get; set; }

        // الشحنة يمكن أن تحتوي على العديد من عناصر الطلب
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
