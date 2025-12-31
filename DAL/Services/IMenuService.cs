using TransportERP.Models.Entities;

namespace TransportERP.Models.Services
{
    public interface IMenuService
    {
        Task<List<Menu>> GetAllMenusAsync();
        Task<List<Menu>> GetUserMenusAsync(int userId);
        Task<UserWiseMenu?> GetUserMenuRightsAsync(int userId, string menuUrl);
        Task<bool> UpdateUserMenuRightsAsync(List<UserWiseMenu> rights);
        Task<List<UserWiseMenu>> GetUserRightsListAsync(int userId);
    }
}
