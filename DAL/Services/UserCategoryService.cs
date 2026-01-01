using Microsoft.EntityFrameworkCore;
using TransportERP.Models.Entities;
using TransportERP.Models.DTOs;
using TransportERP.Models.DbContext;

namespace TransportERP.Models.Services;

public class UserCategoryService : IUserCategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UserCategoryService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<UserCategoryDto>> GetAllUserCategoriesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var users = await context.UserCategories
            .OrderByDescending(u => u.UserCategoryID)
            .ToListAsync();

        return users.Select(MapToViewModel).ToList();
    }

    public async Task<UserCategoryDto?> GetUserCategoryByIdAsync(int UserCategoryID)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.UserCategories
            .FirstOrDefaultAsync(u => u.UserCategoryID == UserCategoryID);
        return user != null ? MapToViewModel(user) : null;
    }

    public async Task<UserCategoryDto> CreateUserCategoryAsync(UserCategoryDto UserCategoryDto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = MapToEntity(UserCategoryDto);

        context.UserCategories.Add(user);
        await context.SaveChangesAsync();

        UserCategoryDto.UserCategoryID = user.UserCategoryID;
        return UserCategoryDto;
    }

    public async Task<UserCategoryDto> UpdateUserCategoryAsync(UserCategoryDto UserCategoryDto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.UserCategories.FindAsync(UserCategoryDto.UserCategoryID);
        if (user == null)
            throw new Exception($"User with ID {UserCategoryDto.UserCategoryID} not found");

        // Update properties
        user.UserCategoryID = UserCategoryDto.UserCategoryID;
        user.UserCategoryName = UserCategoryDto.UserCategoryName;
        user.IsActive = UserCategoryDto.IsActive;

        context.UserCategories.Update(user);
        await context.SaveChangesAsync();

        return UserCategoryDto;
    }

    public async Task<bool> DeleteUserCategoryAsync(int UserCategoryID)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(UserCategoryID);
        if (user == null)
            return false;

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetTotalUserCategoriesCountAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.CountAsync();
    }

    // Mapping methods
    private UserCategoryDto MapToViewModel(UserCategory user)
    {
        return new UserCategoryDto
        {
            UserCategoryID = user.UserCategoryID,
             UserCategoryName = user.UserCategoryName,
            IsActive = user.IsActive ?? true
        };
    }

    private UserCategory MapToEntity(UserCategoryDto viewModel)
    {
        return new UserCategory
        {
            UserCategoryID = viewModel.UserCategoryID,
            UserCategoryName = viewModel.UserCategoryName,
            IsActive = viewModel.IsActive
        };
    }
}

