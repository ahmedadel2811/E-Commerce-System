using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Service.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>,ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Coupon> GetByIdAsync(int id)
        {
            return await _context.Coupons
                .Include(c => c.ProductCoupons)
                .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Coupon> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .Include(c => c.ProductCoupons)
                .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());
        }

        public async Task<IEnumerable<Coupon>> GetAllAsync()
        {
            return await _context.Coupons
                .Include(c => c.ProductCoupons)
                .ThenInclude(pc => pc.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
        {
            return await _context.Coupons
                .Include(c => c.ProductCoupons)
                .ThenInclude(pc => pc.Product)
                .Where(c => c.IsActive && c.ExpiryDate > System.DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<Coupon> AddAsync(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Coupon coupon)
        {
            // Remove product associations first
            var productCoupons = _context.ProductCoupons.Where(pc => pc.CouponId == coupon.Id);
            _context.ProductCoupons.RemoveRange(productCoupons);

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.Coupons.Where(c => c.Code.ToLower() == code.ToLower());

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task AddProductsToCouponAsync(int couponId, List<int> productIds)
        {
            var existingProductCoupons = await _context.ProductCoupons
                .Where(pc => pc.CouponId == couponId && productIds.Contains(pc.ProductId))
                .Select(pc => pc.ProductId)
                .ToListAsync();

            var newProductIds = productIds.Except(existingProductCoupons).ToList();

            foreach (var productId in newProductIds)
            {
                var productCoupon = new ProductCoupon
                {
                    CouponId = couponId,
                    ProductId = productId
                };
                await _context.ProductCoupons.AddAsync(productCoupon);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductsFromCouponAsync(int couponId, List<int> productIds)
        {
            var productCoupons = await _context.ProductCoupons
                .Where(pc => pc.CouponId == couponId && productIds.Contains(pc.ProductId))
                .ToListAsync();

            _context.ProductCoupons.RemoveRange(productCoupons);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetCouponProductsAsync(int couponId)
        {
            return await _context.ProductCoupons
                .Where(pc => pc.CouponId == couponId)
                .Select(pc => pc.Product)
                .ToListAsync();
        }

        public async Task<bool> IsProductInCouponAsync(int couponId, int productId)
        {
            return await _context.ProductCoupons
                .AnyAsync(pc => pc.CouponId == couponId && pc.ProductId == productId);
        }
    }
}