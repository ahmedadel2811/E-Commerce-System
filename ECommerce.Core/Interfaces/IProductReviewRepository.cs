using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IProductReviewRepository : IGenericRepository<ProductReview>
    {
        Task<ProductReview> GetReviewWithDetailsAsync(int id);
        Task<IEnumerable<ProductReview>> GetReviewsByUserAsync(string userId);
        Task<IEnumerable<ProductReview>> GetPendingReviewsAsync();
        Task<IEnumerable<ProductReview>> GetApprovedReviewsAsync(int productId);
        Task<bool> UserHasReviewedProductAsync(string userId, int productId);

        Task<double> GetProductAverageRatingAsync(int productId);

    }
}
