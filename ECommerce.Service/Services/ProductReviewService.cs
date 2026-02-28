using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.ProductReview;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IGenericRepository<AuditLog> _auditLogRepository;
        private readonly ICurrentUserService _currentUserService;

        public ProductReviewService(
            IProductReviewRepository productReviewRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IGenericRepository<AuditLog> auditLogRepository,
            ICurrentUserService currentUserService)
        {
            _productReviewRepository = productReviewRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _auditLogRepository = auditLogRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Response<ProductReviewDetailDto>> GetReviewByIdAsync(int id)
        {
            try
            {
                var review = await _productReviewRepository.GetReviewWithDetailsAsync(id);
                if (review == null)
                    return Response<ProductReviewDetailDto>.Fail("Review not found");

                var reviewDto = MapToProductReviewDetailDto(review);
                return Response<ProductReviewDetailDto>.Success(reviewDto, "Review retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductReviewDetailDto>.Fail($"Error retrieving review: {ex.Message}");
            }
        }

       

        public async Task<Response<IEnumerable<ProductReviewDto>>> GetReviewsByUserAsync(string userId)
        {
            try
            {
                var reviews = await _productReviewRepository.GetReviewsByUserAsync(userId);
                var reviewDtos = reviews.Select(MapToProductReviewDto).ToList();
                return Response<IEnumerable<ProductReviewDto>>.Success(reviewDtos, "User reviews retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ProductReviewDto>>.Fail($"Error retrieving user reviews: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<ProductReviewDto>>> GetPendingReviewsAsync()
        {
            try
            {
                var reviews = await _productReviewRepository.GetPendingReviewsAsync();
                var reviewDtos = reviews.Select(MapToProductReviewDto).ToList();
                return Response<IEnumerable<ProductReviewDto>>.Success(reviewDtos, "Pending reviews retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ProductReviewDto>>.Fail($"Error retrieving pending reviews: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<ProductReviewDto>>> GetApprovedReviewsAsync(int productId)
        {
            try
            {
                var reviews = await _productReviewRepository.GetApprovedReviewsAsync(productId);
                var reviewDtos = reviews.Select(MapToProductReviewDto).ToList();
                return Response<IEnumerable<ProductReviewDto>>.Success(reviewDtos, "Approved reviews retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ProductReviewDto>>.Fail($"Error retrieving approved reviews: {ex.Message}");
            }
        }

        public async Task<Response<ProductReviewDto>> CreateReviewAsync(string userId, CreateProductReviewDto reviewDto)
        {
            try
            {
                // التحقق من وجود المنتج
                var product = await _productRepository.GetByIdAsync(reviewDto.ProductId);
                if (product == null)
                    return Response<ProductReviewDto>.Fail("Product not found");

                // التحقق من صحة التقييم
                if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                    return Response<ProductReviewDto>.Fail("Rating must be between 1 and 5");

                // التحقق من طول التعليق
                if (!string.IsNullOrEmpty(reviewDto.Comment) && reviewDto.Comment.Length > 1000)
                    return Response<ProductReviewDto>.Fail("Comment cannot exceed 1000 characters");

                // التحقق إذا كان المستخدم قدم تقييم للمنتج من قبل
                var hasExistingReview = await _productReviewRepository.UserHasReviewedProductAsync(userId, reviewDto.ProductId);
                if (hasExistingReview)
                    return Response<ProductReviewDto>.Fail("You have already reviewed this product");

                // التحقق إذا كان المستخدم اشترى المنتج (للـ verified purchase)
                var hasPurchasedProduct = await HasUserPurchasedProductAsync(userId, reviewDto.ProductId);

                var review = new ProductReview
                {
                    ProductId = reviewDto.ProductId,
                    UserId = userId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                var createdReview = await _productReviewRepository.AddAsync(review);

                // تحديث متوسط تقييم المنتج
                await UpdateProductAverageRating(reviewDto.ProductId);

                await CreateAuditLog("ProductReview", "Create",
                    $"Review created for product: {product.Name}. Rating: {reviewDto.Rating}",
                    userId);

                var resultDto = MapToProductReviewDto(createdReview);
                resultDto.IsVerifiedPurchase = hasPurchasedProduct;
                return Response<ProductReviewDto>.Success(resultDto, "Review created successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductReviewDto>.Fail($"Error creating review: {ex.Message}");
            }
        }

        public async Task<Response<ProductReviewDto>> UpdateReviewAsync(int id, string userId, UpdateProductReviewDto reviewDto)
        {
            try
            {
                var existingReview = await _productReviewRepository.GetByIdAsync(id);
                if (existingReview == null)
                    return Response<ProductReviewDto>.Fail("Review not found");

                // التحقق من ملكية التقييم
                if (existingReview.UserId != userId && !_currentUserService.Roles.Contains("Admin"))
                    return Response<ProductReviewDto>.Fail("You are not authorized to update this review");

                // التحقق من صحة التقييم
                if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                    return Response<ProductReviewDto>.Fail("Rating must be between 1 and 5");

                // التحقق من طول التعليق
                if (!string.IsNullOrEmpty(reviewDto.Comment) && reviewDto.Comment.Length > 1000)
                    return Response<ProductReviewDto>.Fail("Comment cannot exceed 1000 characters");

                // تحديث الخصائص
                existingReview.Rating = reviewDto.Rating;
                existingReview.Comment = reviewDto.Comment;
                existingReview.UpdatedAt = DateTime.UtcNow;

                await _productReviewRepository.UpdateAsync(existingReview);

                // تحديث متوسط تقييم المنتج
                await UpdateProductAverageRating(existingReview.ProductId);

                await CreateAuditLog("ProductReview", "Update",
                    $"Review updated for product ID: {existingReview.ProductId}",
                    userId);

                var updatedDto = MapToProductReviewDto(existingReview);
                return Response<ProductReviewDto>.Success(updatedDto, "Review updated successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductReviewDto>.Fail($"Error updating review: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeleteReviewAsync(int id, string userId)
        {
            try
            {
                var review = await _productReviewRepository.GetByIdAsync(id);
                if (review == null)
                    return Response<bool>.Fail("Review not found");

                // التحقق من ملكية التقييم أو صلاحيات الأدمن
                if (review.UserId != userId && !_currentUserService.Roles.Contains("Admin"))
                    return Response<bool>.Fail("You are not authorized to delete this review");

                await _productReviewRepository.DeleteAsync(review);

                // تحديث متوسط تقييم المنتج
                await UpdateProductAverageRating(review.ProductId);

                await CreateAuditLog("ProductReview", "Delete",
                    $"Review deleted for product ID: {review.ProductId}",
                    userId);

                return Response<bool>.Success(true, "Review deleted successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error deleting review: {ex.Message}");
            }
        }

       

       

      
      
        private async Task<bool> HasUserPurchasedProductAsync(string userId, int productId)
        {
            // هعمل تحقق إذا كان المستخدم طلب المنتج من قبل 
            var userOrders = await _orderRepository.GetUserOrdersAsync(userId);
            return userOrders.Any(order =>
                order.OrderItems.Any(oi => oi.ProductId == productId) &&
                (order.Status == Core.Enums.OrderStatus.Delivered || order.Status == Core.Enums.OrderStatus.Confirmed));
        }

        private async Task UpdateProductAverageRating(int productId)
        {
            var averageRating = await _productReviewRepository.GetProductAverageRatingAsync(productId);

            
        }

        private async Task CreateAuditLog(string entityName, string actionType, string description, string userId)
        {
            var auditLog = new AuditLog
            {
                EntityName = entityName,
                ActionType = actionType,
                UserId = userId,
                NewValues = description,
                DateTime = DateTime.UtcNow
            };
            await _auditLogRepository.AddAsync(auditLog);
        }

        private ProductReviewDto MapToProductReviewDto(ProductReview review)
        {
            var isApproved = review.CreatedAt <= DateTime.UtcNow.AddHours(-24);

            return new ProductReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                ProductName = review.Product?.Name,
                UserId = review.UserId,
                UserName = review.User?.FullName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                IsVerifiedPurchase = false, //orders ممكن نحسبها من الـ 
                IsApproved = isApproved
            };
        }

        private ProductReviewDetailDto MapToProductReviewDetailDto(ProductReview review)
        {
            var isApproved = review.CreatedAt <= DateTime.UtcNow.AddHours(-24);

            return new ProductReviewDetailDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                ProductName = review.Product?.Name,
                ProductImage = review.Product?.ImageUrl,
                UserId = review.UserId,
                UserName = review.User?.FullName,
                UserEmail = review.User?.Email,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                IsVerifiedPurchase = false, // orders ممكن نحسبها من  
                IsApproved = isApproved
            };
        }
    }
}