using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs.vendor
{
    public class VendorDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ShopName { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public bool IsApproved { get; set; }
        public double RatingAverage { get; set; }
        public decimal TotalSales { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}