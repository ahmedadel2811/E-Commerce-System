using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.order
{
    //  لإنشاء طلب من السلة
    public class CreateOrderFromCartDto
    {
        public decimal ShippingFee { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Wallet;
        public int? AddressId { get; set; }
        public string? CouponCode { get; set; }
    }
}
