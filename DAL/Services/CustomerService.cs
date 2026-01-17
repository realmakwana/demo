using Microsoft.EntityFrameworkCore;
using ERP.Models.DbContext;
using ERP.Models.Entities;

namespace ERP.Models.Services
{
    /// <summary>
    /// Interface for Customer service operations
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Gets all customers from the database
        /// </summary>
        /// <returns>List of all customers</returns>
        Task<List<Customer>> GetAllCustomersAsync();

        /// <summary>
        /// Gets a customer by their ID
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer entity or null if not found</returns>
        Task<Customer?> GetCustomerByIdAsync(int id);

        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <param name="entity">Customer entity to create</param>
        /// <returns>Created customer entity</returns>
        Task<Customer> CreateCustomerAsync(Customer entity);

        /// <summary>
        /// Updates an existing customer
        /// </summary>
        /// <param name="entity">Customer entity to update</param>
        /// <returns>Updated customer entity</returns>
        Task<Customer> UpdateCustomerAsync(Customer entity);

        /// <summary>
        /// Deletes a customer by ID
        /// </summary>
        /// <param name="id">Customer ID to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteCustomerAsync(int id);

        /// <summary>
        /// Gets the total count of all customers
        /// </summary>
        /// <returns>Total customer count</returns>
        Task<int> GetTotalCustomersCountAsync();

        /// <summary>
        /// Gets all active customers
        /// </summary>
        /// <returns>List of active customers</returns>
        Task<List<Customer>> GetActiveCustomersAsync();
    }

    /// <summary>
    /// Implementation of Customer service operations
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly IDbContextFactory<ERPDbContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of the CustomerService class
        /// </summary>
        /// <param name="contextFactory">Database context factory</param>
        public CustomerService(IDbContextFactory<ERPDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Gets all customers from the database
        /// </summary>
        /// <returns>List of all customers</returns>
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Customers.ToListAsync();
        }

        /// <summary>
        /// Gets a customer by their ID
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer entity or null if not found</returns>
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Customers.FindAsync(id);
        }

        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <param name="entity">Customer entity to create</param>
        /// <returns>Created customer entity</returns>
        public async Task<Customer> CreateCustomerAsync(Customer entity)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Customers.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing customer
        /// </summary>
        /// <param name="entity">Customer entity to update</param>
        /// <returns>Updated customer entity</returns>
        public async Task<Customer> UpdateCustomerAsync(Customer entity)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Customers.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes a customer by ID
        /// </summary>
        /// <param name="id">Customer ID to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var customer = await context.Customers.FindAsync(id);
            if (customer == null)
                return false;

            context.Customers.Remove(customer);
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets the total count of all customers
        /// </summary>
        /// <returns>Total customer count</returns>
        public async Task<int> GetTotalCustomersCountAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Customers.CountAsync();
        }

        /// <summary>
        /// Gets all active customers
        /// </summary>
        /// <returns>List of active customers</returns>
        public async Task<List<Customer>> GetActiveCustomersAsync()
        {
            //await using var context = await _contextFactory.CreateDbContextAsync();
            //return await context.Customers.Where(c => c.IsActive).ToListAsync();

            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Customers
                .Where(s => s.IsActive == true)
                .OrderByDescending(s => s.CustomerID)
                .ToListAsync();

        }
    }
}
