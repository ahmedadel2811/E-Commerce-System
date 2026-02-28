using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IPaymentRepository:IGenericRepository<Payment>
    {

        Task<Payment> GetPaymentWithOrderAsync(int id);
        Task<Payment> GetPaymentByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status);
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(string userId);
        Task UpdatePaymentStatusAsync(int paymentId, string status, string? transactionId = null);




    }
}
