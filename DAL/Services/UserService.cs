using Microsoft.EntityFrameworkCore;
using TransportERP.Models.Entities;
using TransportERP.Models.ViewModels;
using TransportERP.Models.DbContext;

namespace TransportERP.Models.Services;

public class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UserService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<UserViewModel>> GetAllUsersAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var users = await context.Users
            .Include(u => u.Company)
            .OrderByDescending(u => u.UserID)  // Newest first
            .ToListAsync();

        return users.Select(MapToViewModel).ToList();
    }

    public async Task<UserViewModel?> GetUserByIdAsync(int userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Users
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.UserID == userId);
        return user != null ? MapToViewModel(user) : null;
    }

    public async Task<UserViewModel> CreateUserAsync(UserViewModel userViewModel)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = MapToEntity(userViewModel);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        userViewModel.UserID = user.UserID;
        return userViewModel;
    }

    public async Task<UserViewModel> UpdateUserAsync(UserViewModel userViewModel)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(userViewModel.UserID);
        if (user == null)
            throw new Exception($"User with ID {userViewModel.UserID} not found");

        // Update properties
        user.UserCategoryID = userViewModel.UserCategoryID;
        user.UserName = userViewModel.UserName;
        user.Password = userViewModel.Password;
        user.EmailID = userViewModel.EmailID;
        user.MobileNo = userViewModel.MobileNo;
        user.CompanyID = userViewModel.CompanyID;
        user.MAC = userViewModel.MAC;
        user.IsActive = userViewModel.IsActive;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        return userViewModel;
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

    public async Task<List<UserViewModel>> GetUsersByRoleAsync(string role)
    {
        // Since Role is not in the new schema, return all users
        // You can modify this based on UserCategoryID if needed
        return await GetAllUsersAsync();
    }

    // Mapping methods
    private UserViewModel MapToViewModel(User user)
    {
        return new UserViewModel
        {
            UserID = user.UserID,
            UserCategoryID = user.UserCategoryID,
            UserName = user.UserName ?? string.Empty,
            Password = user.Password ?? string.Empty,
            EmailID = user.EmailID,
            MobileNo = user.MobileNo,
            CompanyID = user.CompanyID,
            MAC = user.MAC,
            IsActive = user.IsActive ?? true
        };
    }

    private User MapToEntity(UserViewModel viewModel)
    {
        return new User
        {
            UserID = viewModel.UserID,
            UserCategoryID = viewModel.UserCategoryID,
            UserName = viewModel.UserName,
            Password = viewModel.Password,
            EmailID = viewModel.EmailID,
            MobileNo = viewModel.MobileNo,
            CompanyID = viewModel.CompanyID,
            MAC = viewModel.MAC,
            IsActive = viewModel.IsActive
        };
    }
}

