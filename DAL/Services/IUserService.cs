using TransportERP.Models.Entities;
using TransportERP.Models.ViewModels;

namespace TransportERP.Models.Services;

public interface IUserService
{
    Task<List<UserViewModel>> GetAllUsersAsync();
    Task<UserViewModel?> GetUserByIdAsync(int userId);
    Task<UserViewModel> CreateUserAsync(UserViewModel user);
    Task<UserViewModel> UpdateUserAsync(UserViewModel user);
    Task<bool> DeleteUserAsync(int userId);
    Task<int> GetTotalUsersCountAsync();
    Task<List<UserViewModel>> GetUsersByRoleAsync(string role);
}




