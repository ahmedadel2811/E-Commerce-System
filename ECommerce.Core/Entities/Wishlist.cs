namespace ECommerce.Core.Entities
{
    //قائمة الرغبات
    public class Wishlist : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
