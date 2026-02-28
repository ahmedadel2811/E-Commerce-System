using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    //العنوان
    public class Address : BaseEntity
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public bool IsDefault { get; set; }
        //العديد من العناوين يمكن أن تنتمي إلى نفس المستخدم
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
