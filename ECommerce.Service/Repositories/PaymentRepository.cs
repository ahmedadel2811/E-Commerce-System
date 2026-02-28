using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Service.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context) { }

        public async Task<Payment> GetPaymentWithOrderAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Include(p => p.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .Where(p => p.PaymentStatus.ToString() == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetUserPaymentsAsync(string userId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .Where(p => p.Order.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdatePaymentStatusAsync(int paymentId, string status, string? transactionId = null)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                payment.PaymentStatus = (Core.Enums.PaymentStatus)Enum.Parse(typeof(Core.Enums.PaymentStatus), status);

                if (!string.IsNullOrEmpty(transactionId)) // لو هيا مش فاضيه
                    payment.TransactionId = transactionId;

                if (status == "Paid")
                    payment.PaidDate = DateTime.UtcNow;

                payment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        
    }
}
