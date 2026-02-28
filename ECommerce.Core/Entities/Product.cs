using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{

    //المنتج
    public class Product : BaseEntity
    {

        [Required]
        public string Name { get; set; } // اسم المنتج.
        public string? Description { get; set; } // وصف المنتج
        public decimal Price { get; set; }
        public double? DiscountPercent { get; set; } // نسبة الخصم
        public decimal FinalPrice => DiscountPercent.HasValue ?
    Price - (Price * (decimal)(DiscountPercent.Value / 100)) : Price;
        public string? SKU { get; set; } // رمز المنتج.
        public int StockQuantity { get; set; } // الكمية المتاحة.
        public double? Weight { get; set; } // لو المنتج له وزن
        public string? Dimensions { get; set; } // لو المنتج له ابعاد
        public string? ImageUrl { get; set; } // رابط الصورة الرئيسية
        public bool IsActive { get; set; } = true;

        //المنتج ينتمي إلى بائع واحد.
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }

        // Primary category (optional) : يمكن للمنتج أن ينتمي إلى فئة واحدة
        public int? CategoryId { get; set; }
        public Category Category { get; set; }


        // يمكن للمنتج أن يكون في فئات متعددة من خلال جدول وسيط
        // ProductCategory
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        //المنتج يحتوي على العديد من الصور
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        // المنتج يمكن أن يحتوي على متغيرات مثل اللون أو الحجم
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        //المنتج يمكن أن يحتوي على العديد من التقييمات
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        // عنصر الطلب
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // يمكن إضافة المنتج إلى قوائم الرغبات للمستخدمين المتعددين
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

        // Many-to-Many
        // المنتج يمكن أن يكون عليه خصومات من أكواد خصم متعددة عبر جدول وسيط
        // ProductCoupon
        public ICollection<ProductCoupon> ProductCoupons { get; set; } = new List<ProductCoupon>();
    }
}
