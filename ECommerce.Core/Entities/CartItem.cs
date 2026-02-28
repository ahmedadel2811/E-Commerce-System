namespace ECommerce.Core.Entities
{

    //عنصر السلة
    public class CartItem : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; } // الكميه المطلوبه
        public decimal UnitPrice { get; set; } // سعر الوحده

        public decimal SubTotal => Quantity * UnitPrice;
    }
}
