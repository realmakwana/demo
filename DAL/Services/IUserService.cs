using TransportERP.Models.Entities;
using TransportERP.Models.DTOs;

namespace TransportERP.Models.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserDto> CreateUserAsync(UserDto user);
    Task<UserDto> UpdateUserAsync(UserDto user);
    Task<bool> DeleteUserAsync(int userId);
    Task<int> GetTotalUsersCountAsync();
    Task<List<UserDto>> GetUsersByRoleAsync(string role);
}




