using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal FinalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; }
        public DateTime OrderDate { get; set; }
        public string TrackingNumber { get; set; }
    }
}