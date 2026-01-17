using Microsoft.EntityFrameworkCore;
using ERP.Models.DbContext;
using ERP.Models.Entities;

namespace ERP.Models.Services
{
    /// <summary>
    /// Interface for Invoice service operations
    /// </summary>
    public interface IInvoiceService
    {

        Task<List<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber);
        Task<Invoice> CreateInvoiceAsync(Invoice entity);
        Task<Invoice> UpdateInvoiceAsync(Invoice entity);
        Task<bool> DeleteInvoiceAsync(int id);
        Task<int> GetTotalInvoicesCountAsync();
        Task<List<Invoice>> GetInvoicesByCustomerAsync(int customerId);
        Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<string> GenerateInvoiceNumberAsync();
    }
    public class InvoiceService : IInvoiceService
    {
        private readonly IDbContextFactory<ERPDbContext> _contextFactory;
        public InvoiceService(IDbContextFactory<ERPDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Invoices
                .Include(i => i.Items)
                .OrderByDescending(i => i.invoice_date)
                .ToListAsync();
        }
        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);
        }
        public async Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.invoice_no == invoiceNumber);
        }
        public async Task<Invoice> CreateInvoiceAsync(Invoice entity)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            if (entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedDate = DateTime.Now;
            }

            var itemsToInsert = entity.Items.ToList();
            entity.Items = new List<InvoiceItem>();

            context.Invoices.Add(entity);
            await context.SaveChangesAsync(); 

            foreach (var item in itemsToInsert)
            {
                item.InvoiceId = entity.InvoiceID; 
                item.LineItemID = 0;               

                if (item is BaseEntity itemBase)
                {
                    itemBase.CreatedDate = DateTime.Now;
                }
            }

            context.InvoiceItems.AddRange(itemsToInsert);
            await context.SaveChangesAsync();

            entity.Items = itemsToInsert;

            return entity;
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice entity)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var existingInvoice = await context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.InvoiceID == entity.InvoiceID);

            if (existingInvoice != null)
            {
                if (entity is BaseEntity)
                {
                    entity.ModifiedDate = DateTime.Now;
                }

                context.InvoiceItems.RemoveRange(existingInvoice.Items);
                context.Entry(existingInvoice).CurrentValues.SetValues(entity);

                foreach (var item in entity.Items)
                {
                    item.InvoiceId = entity.InvoiceID; 
                    item.LineItemID = 0;              

                    if (item is BaseEntity itemBase)
                    {
                        itemBase.CreatedDate = DateTime.Now;
                        itemBase.IsActive = true;
                    }
                    context.InvoiceItems.Add(item);
                }

                await context.SaveChangesAsync();
            }

            return entity;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var invoice = await context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null) return false;

            context.Invoices.Remove(invoice);

            await context.SaveChangesAsync();
            return true;
        }
        public async Task<int> GetTotalInvoicesCountAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Invoices.CountAsync();
        }
        public async Task<List<Invoice>> GetInvoicesByCustomerAsync(int customerId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Invoices
                .Include(i => i.Items)
                .Where(i => i.CustomerID == customerId)
                .OrderByDescending(i => i.invoice_date)
                .ToListAsync();
        }
        public async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Invoices
                .Include(i => i.Items)
                .Where(i => i.invoice_date >= startDate && i.invoice_date <= endDate)
                .OrderByDescending(i => i.invoice_date)
                .ToListAsync();
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var lastInvoice = await context.Invoices
                .OrderByDescending(i => i.InvoiceID)
                .FirstOrDefaultAsync();

            var nextNumber = (lastInvoice?.InvoiceID ?? 0) + 1;
            return $"INV-{DateTime.Now:yyyyMMdd}-{nextNumber:D4}";
        }
    }
}
