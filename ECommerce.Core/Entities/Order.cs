using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal TotalAmount { get; set; } //  المبلغ الإجمالي قبل الخصم
        public decimal DiscountAmount { get; set; } //مبلغ الخصم
        //رسوم الشحن
        public decimal ShippingFee { get; set; } // رسوم الشحن
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Wallet; // طريقة الدفع
        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Pending, Confirmed, Shipped, Delivered, Cancelled, Refunded
        //رقم التتبع
        public string? TrackingNumber { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        //   إضافة خاصية للرقم المرجعي
        // في Order Entity - نضيف:
        public string OrderNumber { get; set; } = GenerateOrderNumber();
        public decimal FinalAmount => TotalAmount - DiscountAmount + ShippingFee; // المبلغ النهائى بعد الخصم والتوصيل
        //   كل طلب يحتوي على العديد من العناصر منتجات يعنى
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // one to one  كل طلب يحتوي على شحنة واحدة
        public int? ShipmentId { get; set; }

        public Shipment Shipment { get; set; } 
        // one to one   كل طلب يتضمن عملية دفع واحدة
        public Payment Payment { get; set; }



        private static string GenerateOrderNumber()
        {
            return $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }
    }

}
