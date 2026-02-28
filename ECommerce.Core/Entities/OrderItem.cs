using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    public class OrderItem : BaseEntity
    {

        //الكمية المطلوبة
        public int Quantity { get; set; }
        // سعر الوحدة
        public decimal UnitPrice { get; set; }

        // الاجمالى
        public decimal SubTotal => Quantity * UnitPrice;

        //  كل عنصر ينتمي إلى طلب واحد.
        public int OrderId { get; set; }
        public Order Order { get; set; }
        //كل عنصر ينتمي إلى منتج واحد
        public int ProductId { get; set; }
        public Product Product { get; set; }


        // كل عنصر يمكن ربطه بشحنة
        public int? ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
    }
}
