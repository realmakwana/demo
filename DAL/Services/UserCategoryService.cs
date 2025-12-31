using Microsoft.EntityFrameworkCore;
using TransportERP.Models.Entities;
using TransportERP.Models.ViewModels;
using TransportERP.Models.DbContext;

namespace TransportERP.Models.Services;

public class UserCategoryService : IUserCategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UserCategoryService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<UserCategoryViewModel>> GetAllUserCategoriesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var users = await context.UserCategories
            .OrderByDescending(u => u.UserCategoryID)
            .ToListAsync();

        return users.Select(MapToViewModel).ToList();
    }

    public async Task<UserCategoryViewModel?> GetUserCategoryByIdAsync(int UserCategoryID)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.UserCategories
            .FirstOrDefaultAsync(u => u.UserCategoryID == UserCategoryID);
        return user != null ? MapToViewModel(user) : null;
    }

    public async Task<UserCategoryViewModel> CreateUserCategoryAsync(UserCategoryViewModel UserCategoryViewModel)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = MapToEntity(UserCategoryViewModel);

        context.UserCategories.Add(user);
        await context.SaveChangesAsync();

        UserCategoryViewModel.UserCategoryID = user.UserCategoryID;
        return UserCategoryViewModel;
    }

    public async Task<UserCategoryViewModel> UpdateUserCategoryAsync(UserCategoryViewModel UserCategoryViewModel)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.UserCategories.FindAsync(UserCategoryViewModel.UserCategoryID);
        if (user == null)
            throw new Exception($"User with ID {UserCategoryViewModel.UserCategoryID} not found");

        // Update properties
        user.UserCategoryID = UserCategoryViewModel.UserCategoryID;
        user.UserCategoryName = UserCategoryViewModel.UserCategoryName;
        user.IsActive = UserCategoryViewModel.IsActive;

        context.UserCategories.Update(user);
        await context.SaveChangesAsync();

        return UserCategoryViewModel;
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
    private UserCategoryViewModel MapToViewModel(UserCategory user)
    {
        return new UserCategoryViewModel
        {
            UserCategoryID = user.UserCategoryID,
             UserCategoryName = user.UserCategoryName,
            IsActive = user.IsActive ?? true
        };
    }

    private UserCategory MapToEntity(UserCategoryViewModel viewModel)
    {
        return new UserCategory
        {
            UserCategoryID = viewModel.UserCategoryID,
            UserCategoryName = viewModel.UserCategoryName,
            IsActive = viewModel.IsActive
        };
    }
}

