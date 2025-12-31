using TransportERP.Models.ViewModels;

namespace TransportERP.Models.Services;

public interface IUserCategoryService
{
    Task<List<UserCategoryViewModel>> GetAllUserCategoriesAsync();
    Task<UserCategoryViewModel?> GetUserCategoryByIdAsync(int userId);
    Task<UserCategoryViewModel> CreateUserCategoryAsync(UserCategoryViewModel user);
    Task<UserCategoryViewModel> UpdateUserCategoryAsync(UserCategoryViewModel user);
    Task<bool> DeleteUserCategoryAsync(int userId);
    Task<int> GetTotalUserCategoriesCountAsync();
}




