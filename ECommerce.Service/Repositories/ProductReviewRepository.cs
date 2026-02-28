using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Service.Repositories
{
    public class ProductReviewRepository : GenericRepository<ProductReview>, IProductReviewRepository
    {
        public ProductReviewRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<ProductReview> GetReviewWithDetailsAsync(int id)
        {
            return await _context.ProductReviews
                .Include(pr => pr.Product)
                    .ThenInclude(p => p.Vendor)
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

     
        public async Task<IEnumerable<ProductReview>> GetReviewsByUserAsync(string userId)
        {
            return await _context.ProductReviews
                .Include(pr => pr.Product)
                    .ThenInclude(p => p.Vendor)
                .Where(pr => pr.UserId == userId)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductReview>> GetPendingReviewsAsync()
        {
            return await _context.ProductReviews
                .Include(pr => pr.Product)
                .Include(pr => pr.User)
                .Where(pr => pr.CreatedAt >= DateTime.UtcNow.AddHours(-24)) // Reviews in last 24 hours
                .OrderBy(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductReview>> GetApprovedReviewsAsync(int productId)
        {
            return await _context.ProductReviews
                .Include(pr => pr.User)
                .Where(pr => pr.ProductId == productId && pr.CreatedAt <= DateTime.UtcNow.AddHours(-24))
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetProductAverageRatingAsync(int productId)
        {
            var reviews = await _context.ProductReviews
                .Where(pr => pr.ProductId == productId && pr.CreatedAt <= DateTime.UtcNow.AddHours(-24))
                .ToListAsync();

            return reviews.Any() ? reviews.Average(pr => pr.Rating) : 0;
        }

       

        public async Task<bool> UserHasReviewedProductAsync(string userId, int productId)
        {
            return await _context.ProductReviews
                .AnyAsync(pr => pr.UserId == userId && pr.ProductId == productId);
        }

       
    }
}