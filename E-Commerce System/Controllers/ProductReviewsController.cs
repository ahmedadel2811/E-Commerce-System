using ECommerce.Core.DTOs.ProductReview;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace E_Commerce_System.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IProductReviewService _productReviewService;
        private readonly ICurrentUserService _currentUserService;

        private readonly IMemoryCache _memoryCache;

        public ProductReviewsController(
            IProductReviewService productReviewService,
            ICurrentUserService currentUserService
,
            IMemoryCache memoryCache)
        {
            _productReviewService = productReviewService;
            _currentUserService = currentUserService;
            _memoryCache = memoryCache;
        }

       

        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<Response<IEnumerable<ProductReviewDto>>>> GetMyReviews()
        {
            var result = await _productReviewService.GetReviewsByUserAsync(_currentUserService.UserId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // أي حد يقدر يشوف التقييم
        public async Task<ActionResult<Response<ProductReviewDetailDto>>> GetReview(int id)
        {
            var result = await _productReviewService.GetReviewByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

       

       


     

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Response<ProductReviewDto>>> CreateReview(CreateProductReviewDto reviewDto)
        {
            // Rate limiting للتقييمات
            var rateLimitCheck = await CheckReviewRateLimit();
            if (!rateLimitCheck)
            {
                return StatusCode(429, new
                {
                    Succeeded = false,
                    Message = "Too many review requests. Please try again in 10 seconds.",
                    Errors = new List<string> { "Rate limit exceeded" }
                });
            }

            var result = await _productReviewService.CreateReviewAsync(_currentUserService.UserId, reviewDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetReview), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Response<ProductReviewDto>>> UpdateReview(int id, UpdateProductReviewDto reviewDto)
        {
            var result = await _productReviewService.UpdateReviewAsync(id, _currentUserService.UserId, reviewDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Response<bool>>> DeleteReview(int id)
        {
            var result = await _productReviewService.DeleteReviewAsync(id, _currentUserService.UserId);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

      

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<IEnumerable<ProductReviewDto>>>> GetUserReviews(string userId)
        {
            var result = await _productReviewService.GetReviewsByUserAsync(userId);
            return Ok(result);
        }

        private async Task<bool> CheckReviewRateLimit()
        {
            var cacheKey = $"review_rate_limit_{_currentUserService.UserId}";

            // حاول تجيب آخر وقت
            // review
            if (_memoryCache.TryGetValue(cacheKey, out DateTime lastReviewTime))
            {
                if (DateTime.UtcNow - lastReviewTime < TimeSpan.FromSeconds(10))
                {
                    return false;
                }
            }

            // خزن الوقت الحالي لمدة 10 ثواني
            _memoryCache.Set(cacheKey, DateTime.UtcNow, TimeSpan.FromSeconds(10));
            return true;
        }
    }
}