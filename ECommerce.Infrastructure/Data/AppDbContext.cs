using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<DeliveryAgent> DeliveryAgents { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Person> Persons { get; set; }

        // join tables
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductCoupon> ProductCoupons { get; set; }
        public DbSet<VendorCategory> VendorCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔧 Configure decimal precision for ALL decimal properties automatically
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            // 👤 ApplicationUser Relations
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Vendors)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🏪 Category self reference
            builder.Entity<Category>()
                .HasMany(c => c.SubCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 Composite Keys for Join Tables
            builder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            builder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductCoupon>()
                .HasKey(pc => new { pc.ProductId, pc.CouponId });

            builder.Entity<ProductCoupon>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCoupons)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductCoupon>()
                .HasOne(pc => pc.Coupon)
                .WithMany(c => c.ProductCoupons)
                .HasForeignKey(pc => pc.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VendorCategory>()
                .HasKey(vc => new { vc.VendorId, vc.CategoryId });

            builder.Entity<VendorCategory>()
                .HasOne(vc => vc.Vendor)
                .WithMany(v => v.VendorCategories)
                .HasForeignKey(vc => vc.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VendorCategory>()
                .HasOne(vc => vc.Category)
                .WithMany(c => c.VendorCategories)
                .HasForeignKey(vc => vc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🛒 Cart & Wishlist (Fixed Cascade Issues)
            builder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasOne(w => w.Product)
                .WithMany(p => p.Wishlists)
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 📦 Order Relationships
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🚚 Shipment Relationships
            builder.Entity<Shipment>()
                .HasOne(s => s.Order)
                .WithOne(o => o.Shipment)
                .HasForeignKey<Shipment>(s => s.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Shipment>()
                .HasOne(s => s.DeliveryAgent)
                .WithMany(d => d.Shipments)
                .HasForeignKey(s => s.DeliveryAgentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Shipment>()
                .HasMany(s => s.OrderItems)
                .WithOne(oi => oi.Shipment)
                .HasForeignKey(oi => oi.ShipmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // 💳 Payment - Order (one-to-one)
            builder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 💰 Wallet Relationships
            builder.Entity<Wallet>()
                .HasOne(w => w.User)
                .WithOne(u => u.Wallet)
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WalletTransaction>()
                .HasOne(wt => wt.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(wt => wt.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            // 📸 Product Related Entities
            builder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductVariant>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ⭐ ProductReview (FIXED - Restrict instead of Cascade)
            builder.Entity<ProductReview>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ تم التغيير هنا

            builder.Entity<ProductReview>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // 🚚 DeliveryAgent
            builder.Entity<DeliveryAgent>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🎯 Performance Indexes
            builder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            builder.Entity<Product>()
                .HasIndex(p => new { p.Name, p.IsActive });

            builder.Entity<Product>()
                .HasIndex(p => p.VendorId);

            builder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            builder.Entity<Order>()
                .HasIndex(o => new { o.UserId, o.Status });

            builder.Entity<Order>()
                .HasIndex(o => o.OrderDate);

            builder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();

            builder.Entity<ProductReview>()
                .HasIndex(pr => new { pr.ProductId, pr.CreatedAt });

            builder.Entity<Category>()
                .HasIndex(c => c.ParentCategoryId);

            builder.Entity<Vendor>()
                .HasIndex(v => v.UserId);

            // 📝 Configure Enum Conversions
            builder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<Payment>()
                .Property(p => p.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<Payment>()
                .Property(p => p.PaymentGateway)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<WalletTransaction>()
                .Property(wt => wt.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<DeliveryAgent>()
                .Property(d => d.CurrentStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<Notification>()
                .Property(n => n.Type)
                .HasConversion<string>()
                .HasMaxLength(30);
        }
    }
}
