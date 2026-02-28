namespace ECommerce.Core.DTOs.category
{
    public class CategoryWithSubCategoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<CategoryDto> SubCategories { get; set; } = new();
    }
}
