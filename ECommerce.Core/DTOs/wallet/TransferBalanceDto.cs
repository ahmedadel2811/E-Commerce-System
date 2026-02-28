using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs.wallet
{
    public class TransferBalanceDto
    {
        public string ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
