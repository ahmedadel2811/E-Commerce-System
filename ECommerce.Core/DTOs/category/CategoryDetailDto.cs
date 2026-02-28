using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.product;

namespace ECommerce.Core.DTOs.category
{
    public class CategoryDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public List<CategoryDto> SubCategories { get; set; } = new();
        public List<ProductDto> Products { get; set; } = new();
    }
}
