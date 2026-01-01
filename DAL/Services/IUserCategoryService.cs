using TransportERP.Models.DTOs;

namespace TransportERP.Models.Services;

public interface IUserCategoryService
{
    Task<List<UserCategoryDto>> GetAllUserCategoriesAsync();
    Task<UserCategoryDto?> GetUserCategoryByIdAsync(int userId);
    Task<UserCategoryDto> CreateUserCategoryAsync(UserCategoryDto user);
    Task<UserCategoryDto> UpdateUserCategoryAsync(UserCategoryDto user);
    Task<bool> DeleteUserCategoryAsync(int userId);
    Task<int> GetTotalUserCategoriesCountAsync();
}




