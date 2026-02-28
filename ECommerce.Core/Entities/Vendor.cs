using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    //البائع
    public class Vendor : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string ShopName { get; set; } //اسم المتجر
        public string Description { get; set; }
        public string? LogoUrl { get; set; } // رابط الشعار
        public bool IsApproved { get; set; }
        public double RatingAverage { get; set; } // التقييم المتوسط للبائع
        public decimal TotalSales { get; set; } // إجمالي المبيعات.

        // البائع لديه العديد من المنتجات
        public ICollection<Product> Products { get; set; } = new List<Product>();

        // يمكن للبائع بيع منتجات في العديد من الفئات عبر جدول وسيط
        // VendorCategory
        public ICollection<VendorCategory> VendorCategories { get; set; } = new List<VendorCategory>();
    }
}
