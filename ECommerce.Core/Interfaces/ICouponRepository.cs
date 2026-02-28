using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface ICouponRepository : IGenericRepository<Coupon>
    {
        Task<Coupon> GetByIdAsync(int id);
        Task<Coupon> GetByCodeAsync(string code);
        Task<IEnumerable<Coupon>> GetAllAsync();
        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();
        Task<Coupon> AddAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task DeleteAsync(Coupon coupon);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task AddProductsToCouponAsync(int couponId, List<int> productIds);
        Task RemoveProductsFromCouponAsync(int couponId, List<int> productIds);
        Task<List<Product>> GetCouponProductsAsync(int couponId);
        Task<bool> IsProductInCouponAsync(int couponId, int productId);
    }
}
