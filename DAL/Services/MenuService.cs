using Microsoft.EntityFrameworkCore;
using TransportERP.Models.DbContext;
using TransportERP.Models.Entities;

namespace TransportERP.Models.Services
{
    public class MenuService : IMenuService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public MenuService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Menu>> GetAllMenusAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Menus
                .OrderBy(m => m.Sequence)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetUserMenusAsync(int userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            // Fetch menus where the user has IsShow = true
            var allowedMenuIds = await context.UserWiseMenus
                .Where(u => u.UserID == userId && u.IsShow && u.IsActive)
                .Select(u => u.MenuID)
                .ToListAsync();

            var allMenus = await context.Menus
                .Where(m => m.IsActive)
                .OrderBy(m => m.Sequence)
                .ToListAsync();

            // Filter menus that are in allowed list
            // We also need to include parents even if they aren't explicitly in the allowed list but have allowed children
            // (Standard ERP behavior)
            
            return allMenus.Where(m => allowedMenuIds.Contains(m.MenuID) || 
                                      allMenus.Any(child => child.ParentMenuID == m.MenuID && allowedMenuIds.Contains(child.MenuID)))
                           .ToList();
        }

        public async Task<UserWiseMenu?> GetUserMenuRightsAsync(int userId, string menuUrl)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            // Normalize URL (strip leading slash, etc.)
            var normalizedUrl = menuUrl.TrimStart('/');
            
            return await context.UserWiseMenus
                .Include(u => u.Menu)
                .FirstOrDefaultAsync(u => u.UserID == userId && 
                                         (u.Menu!.MenuUrl == normalizedUrl || u.Menu.MenuUrl == "/" + normalizedUrl) && 
                                         u.IsActive);
        }

        public async Task<List<UserWiseMenu>> GetUserRightsListAsync(int userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserWiseMenus
                .Include(u => u.Menu)
                .Where(u => u.UserID == userId && u.IsActive)
                .ToListAsync();
        }

        public async Task<bool> UpdateUserMenuRightsAsync(List<UserWiseMenu> rights)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                foreach (var right in rights)
                {
                    if (right.UserWiseMenuID > 0)
                    {
                        context.UserWiseMenus.Update(right);
                    }
                    else
                    {
                        await context.UserWiseMenus.AddAsync(right);
                    }
                }
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
