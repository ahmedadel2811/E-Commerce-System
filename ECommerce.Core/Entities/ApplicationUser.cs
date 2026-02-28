using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Core.Entities
{

    // المستخدم
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }


        // Navigation

        //كل مستخدم له عدة طلبات
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        //كل مستخدم لديه سلة مشتريات تحتوي على عدة منتجات
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        //كل مستخدم يحتوي على محفظة
        public Wallet Wallet { get; set; }
        // يمكن للمستخدم إضافة تقييمات لمنتجات متعددة
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        //يمكن للمستخدم تلقي إشعارات
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        // المستخدم ممكن يكون له اكتر من عنوان
        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        // ممكن المستخدم يكون بائع
        public ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();


        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    }
}
