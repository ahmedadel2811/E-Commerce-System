using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    public class ProductCoupon
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int CouponId { get; set; }
        public Coupon Coupon { get; set; }
    }
}
