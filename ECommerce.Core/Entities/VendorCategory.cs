using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    public class VendorCategory
    {
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
