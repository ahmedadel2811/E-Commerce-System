using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    //الفئة 
    public class Category : BaseEntity
    {
        public string Name { get; set; } // اسم الفئة
        public string? Description { get; set; } // وصف الفئة
        public string? ImageUrl { get; set; } // رابط الصورة

        // فئة الأصل التي تنتمي إليها هذه الفئة
        // (Self-Referencing Relationship)
        public int? ParentCategoryId { get; set; }  //   يشير إلى الفئة الأصلية

        // الفئة الأصلية نفسها (Self-Referencing Relationship)
        public Category ParentCategory { get; set; }

        // مجموعة الفئات الفرعية (Subcategories)
        // التي تنتمي إلى هذه الفئة
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        // مجموعة المنتجات التي تنتمي إلى هذه الفئة
        public ICollection<Product> Products { get; set; } = new List<Product>();

        // جدول وسيط يربط بين المنتجات والفئات
        // (Many-to-Many Relationship)
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        // جدول وسيط يربط بين البائعين والفئات
        // (Many-to-Many Relationship)
        public ICollection<VendorCategory> VendorCategories { get; set; } = new List<VendorCategory>();
    }
}
