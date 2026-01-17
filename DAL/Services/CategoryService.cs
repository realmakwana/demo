using Microsoft.EntityFrameworkCore;
using ERP.Models.DbContext;
using ERP.Models.Entities;

namespace ERP.Models.Services
{
    /// <summary>
    /// Interface for Category service operations
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Gets all Categorys from the database
        /// </summary>
        /// <returns>List of all Categorys</returns>
        Task<List<Category>> GetAllCategorysAsync();

        /// <summary>
        /// Gets a Category by their ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category entity or null if not found</returns>
        Task<Category?> GetCategoryByIdAsync(int id);

        /// <summary>
        /// Creates a new Category
        /// </summary>
        /// <param name="entity">Category entity to create</param>
        /// <returns>Created Category entity</returns>
        Task<Category> CreateCategoryAsync(Category entity);

        /// <summary>
        /// Updates an existing Category
        /// </summary>
        /// <param name="entity">Category entity to update</param>
        /// <returns>Updated Category entity</returns>
        Task<Category> UpdateCategoryAsync(Category entity);

        /// <summary>
        /// Deletes a Category by ID
        /// </summary>
        /// <param name="id">Category ID to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteCategoryAsync(int id);

        /// <summary>
        /// Gets the total count of all Categorys
        /// </summary>
        /// <returns>Total Category count</returns>
        Task<int> GetTotalCategorysCountAsync();

        /// <summary>
        /// Gets all active Categorys
        /// </summary>
        /// <returns>List of active Categorys</returns>
        Task<List<Category>> GetActiveCategorysAsync();
    }

    /// <summary>
    /// Implementation of Category service operations
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IDbContextFactory<ERPDbContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of the CategoryService class
        /// </summary>
        /// <param name="contextFactory">Database context factory</param>
        public CategoryService(IDbContextFactory<ERPDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Gets all Categorys from the database
        /// </summary>
        /// <returns>List of all Categorys</returns>
        public async Task<List<Category>> GetAllCategorysAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Category.ToListAsync();
        }

        /// <summary>
        /// Gets a Category by their ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category entity or null if not found</returns>
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Category.FindAsync(id);
        }

        /// <summary>
        /// Creates a new Category
        /// </summary>
        /// <param name="entity">Category entity to create</param>
        /// <returns>Created Category entity</returns>
        public async Task<Category> CreateCategoryAsync(Category entity)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Category.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing Category
        /// </summary>
        /// <param name="entity">Category entity to update</param>
        /// <returns>Updated Category entity</returns>
        public async Task<Category> UpdateCategoryAsync(Category entity)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Category.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes a Category by ID
        /// </summary>
        /// <param name="id">Category ID to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var Category = await context.Category.FindAsync(id);
            if (Category == null)
                return false;

            context.Category.Remove(Category);
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets the total count of all Categorys
        /// </summary>
        /// <returns>Total Category count</returns>
        public async Task<int> GetTotalCategorysCountAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Category.CountAsync();
        }

        /// <summary>
        /// Gets all active Categorys
        /// </summary>
        /// <returns>List of active Categorys</returns>
        public async Task<List<Category>> GetActiveCategorysAsync()
        {
            //await using var context = await _contextFactory.CreateDbContextAsync();
            //return await context.Categorys.Where(c => c.IsActive).ToListAsync();

            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Category
                .Where(s => s.IsActive == true)
                .OrderByDescending(s => s.CatID)
                .ToListAsync();

        }
    }
}
