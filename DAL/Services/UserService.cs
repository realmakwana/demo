using Microsoft.EntityFrameworkCore;
using ERP.Models.Entities;
using ERP.Models.DbContext;

namespace ERP.Models.Services;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int userId);
    Task<int> GetTotalUsersCountAsync();
    Task<List<User>> GetUsersByRoleAsync(string role);
}

public class UserService : IUserService
{
    private readonly IDbContextFactory<ERPDbContext> _contextFactory;

    public UserService(IDbContextFactory<ERPDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .Include(u => u.Company)
            .OrderByDescending(u => u.UserID)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.UserID == userId);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        context.Users.Update(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return false;

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.CountAsync();
    }

    public async Task<List<User>> GetUsersByRoleAsync(string role)
    {
        // Since Role is not in the new schema, return all users
        // You can modify this based on UserCategoryID if needed
        return await GetAllUsersAsync();
    }
}
