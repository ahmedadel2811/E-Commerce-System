namespace ECommerce.Core.DTOs.ProductReview
{
    public class CreateProductReviewDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}