using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.ProductReview;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface IProductReviewService
    {
        Task<Response<ProductReviewDetailDto>> GetReviewByIdAsync(int id);
        Task<Response<IEnumerable<ProductReviewDto>>> GetReviewsByUserAsync(string userId);
        Task<Response<IEnumerable<ProductReviewDto>>> GetPendingReviewsAsync();
        Task<Response<IEnumerable<ProductReviewDto>>> GetApprovedReviewsAsync(int productId);
        Task<Response<ProductReviewDto>> CreateReviewAsync(string userId, CreateProductReviewDto reviewDto);
        Task<Response<ProductReviewDto>> UpdateReviewAsync(int id, string userId, UpdateProductReviewDto reviewDto);
        Task<Response<bool>> DeleteReviewAsync(int id, string userId);
    }
}
