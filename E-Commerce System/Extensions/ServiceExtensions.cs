using AutoMapper;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Profiles;
using ECommerce.Service.Repositories;
using ECommerce.Service.Services;

namespace E_Commerce_System.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            //// Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();


            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IGenericRepository<OrderItem>, GenericRepository<OrderItem>>();

            services.AddScoped<ICartRepository, CartRepository>();


            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IGenericRepository<WalletTransaction>, GenericRepository<WalletTransaction>>();
            services.AddScoped<IGenericRepository<ApplicationUser>, GenericRepository<ApplicationUser>>();


            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IGenericRepository<AuditLog>, GenericRepository<AuditLog>>();

            services.AddScoped<IShipmentRepository, ShipmentRepository>();

            services.AddScoped<IDeliveryAgentRepository, DeliveryAgentRepository>();

            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

            services.AddScoped<IVendorRepository, VendorRepository>();

            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();


            // Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IShipmentService, ShipmentService>();

            services.AddScoped<IDeliveryAgentService, DeliveryAgentService>();


            services.AddScoped<IProductReviewService, ProductReviewService>();

            services.AddScoped<IVendorService, VendorService>();

            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<ICouponService, CouponService>();

        }
    }
}