using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    // الكوبون
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } // رمز الكوبون
        public decimal? DiscountAmount { get; set; } // مبلغ الخصم
        public double? DiscountPercent { get; set; } // نسبة الخصم
        public DateTime ExpiryDate { get; set; } // تاريخ انتهاء الصلاحية
        public int UsageLimit { get; set; } = 0; //  الحد الأقصى لاستخدام الكوبون
        public bool IsActive { get; set; } = true;  // هل صالح ولا لا


        // الكوبون يمكن أن ينطبق على منتجات متعددة
        // من خلال جدول وسيط
        // ProductCoupon
        public ICollection<ProductCoupon> ProductCoupons { get; set; } = new List<ProductCoupon>();
    }
}
