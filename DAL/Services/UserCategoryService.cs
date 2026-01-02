using Microsoft.EntityFrameworkCore;
using ERP.Models.Entities;
using ERP.Models.DbContext;

namespace ERP.Models.Services;

public interface IUserCategoryService
{
    Task<List<UserCategory>> GetAllUserCategoriesAsync();
    Task<UserCategory?> GetUserCategoryByIdAsync(int userId);
    Task<UserCategory> CreateUserCategoryAsync(UserCategory user);
    Task<UserCategory> UpdateUserCategoryAsync(UserCategory user);
    Task<bool> DeleteUserCategoryAsync(int userId);
    Task<int> GetTotalUserCategoriesCountAsync();
}

public class UserCategoryService : IUserCategoryService
{
    private readonly IDbContextFactory<ERPDbContext> _contextFactory;

    public UserCategoryService(IDbContextFactory<ERPDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<UserCategory>> GetAllUserCategoriesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UserCategories
            .OrderByDescending(u => u.UserCategoryID)
            .ToListAsync();
    }

    public async Task<UserCategory?> GetUserCategoryByIdAsync(int UserCategoryID)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UserCategories
            .FirstOrDefaultAsync(u => u.UserCategoryID == UserCategoryID);
    }

    public async Task<UserCategory> CreateUserCategoryAsync(UserCategory userCategory)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.UserCategories.Add(userCategory);
        await context.SaveChangesAsync();

        return userCategory;
    }

    public async Task<UserCategory> UpdateUserCategoryAsync(UserCategory userCategory)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        context.UserCategories.Update(userCategory);
        await context.SaveChangesAsync();

        return userCategory;
    }

    public async Task<bool> DeleteUserCategoryAsync(int UserCategoryID)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var userCategory = await context.UserCategories.FindAsync(UserCategoryID);
        if (userCategory == null)
            return false;

        context.UserCategories.Remove(userCategory);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetTotalUserCategoriesCountAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UserCategories.CountAsync();
    }
}
