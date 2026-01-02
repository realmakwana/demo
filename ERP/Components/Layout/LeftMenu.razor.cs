using Microsoft.AspNetCore.Components;
using ERP.Models.Entities;
using ERP.Models.Services;

namespace ERP.Components.Layout
{
    public partial class LeftMenu : ComponentBase, IDisposable
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;

        [Parameter] public bool IsCollapsed { get; set; }
        [Parameter] public bool IsMobileOpen { get; set; }
        [Parameter] public EventCallback OnCloseMobileMenu { get; set; }
        [Parameter] public EventCallback OnToggleSidebar { get; set; }

        private List<Menu> menus = new();
        private Dictionary<int, bool> submenuStates = new();
        private bool isLoading = true;
        private bool _disposed = false;
        private readonly SemaphoreSlim _loadMenusSemaphore = new(1, 1);
        private Action? _authStateChangedHandler;

        protected override async Task OnInitializedAsync()
        {
            await LoadMenus();
            
            // Store the handler reference so we can unsubscribe later
            _authStateChangedHandler = async () =>
            {
                if (!_disposed)
                {
                    await LoadMenus();
                    await InvokeAsync(StateHasChanged);
                }
            };
            
            AuthService.OnAuthStateChanged += _authStateChangedHandler;
        }

        private async Task LoadMenus()
        {
            // Check if disposed
            if (_disposed) return;

            // Prevent concurrent execution
            try
            {
                await _loadMenusSemaphore.WaitAsync();
                
                // Double-check after acquiring lock
                if (_disposed) return;

                try
                {
                    isLoading = true;
                    if (AuthService.IsAuthenticated)
                    {
                        menus = await MenuService.GetUserMenusAsync(AuthService.UserId);
                        
                        // Preserve existing submenu states and auto-expand active parents
                        var currentPath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
                        
                        foreach (var menu in menus.Where(m => m.ParentMenuID == 0))
                        {
                            // If this menu doesn't have a state yet, initialize it
                            if (!submenuStates.ContainsKey(menu.MenuID))
                            {
                                // Check if any child is active - if yes, auto-expand
                                var shouldExpand = IsParentOfActivePage(menu.MenuID);
                                submenuStates[menu.MenuID] = shouldExpand;
                            }
                            // If state exists, preserve it (don't reset to false)
                        }
                        
                        // Also check for nested submenus (level 2 with children)
                        foreach (var menu in menus.Where(m => m.ParentMenuID != 0))
                        {
                            var hasChildren = menus.Any(m => m.ParentMenuID == menu.MenuID);
                            if (hasChildren && !submenuStates.ContainsKey(menu.MenuID))
                            {
                                var shouldExpand = IsParentOfActivePage(menu.MenuID);
                                submenuStates[menu.MenuID] = shouldExpand;
                            }
                        }
                    }
                    else
                    {
                        menus = new();
                    }
                    isLoading = false;
                }
                finally
                {
                    _loadMenusSemaphore.Release();
                }
            }
            catch (ObjectDisposedException)
            {
                // Semaphore was disposed, component is being torn down
                // This is expected during disposal, just return
                return;
            }
        }

        private string GetActiveClass(string? path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            var currentPath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            if (string.IsNullOrEmpty(currentPath) && (path == "/" || path == "")) return "active";
            return currentPath == path.TrimStart('/') ? "active" : "";
        }

        private bool IsParentOfActivePage(int menuId)
        {
            var currentPath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            if (string.IsNullOrEmpty(currentPath)) return false;

            // Recursive function to check all descendants
            bool CheckDescendants(int parentId)
            {
                var children = menus.Where(m => m.ParentMenuID == parentId);
                
                foreach (var child in children)
                {
                    // Check if this child's URL matches current page
                    if (!string.IsNullOrEmpty(child.MenuUrl) && 
                        currentPath == child.MenuUrl.TrimStart('/'))
                    {
                        return true;
                    }
                    
                    // Recursively check this child's children (grandchildren, etc.)
                    if (CheckDescendants(child.MenuID))
                    {
                        return true;
                    }
                }
                
                return false;
            }

            return CheckDescendants(menuId);
        }

        private string GetParentActiveClass(int menuId)
        {
            return IsParentOfActivePage(menuId) ? "parent-active" : "";
        }

        private void ToggleSubmenu(int menuId)
        {
            if (!IsCollapsed)
            {
                if (submenuStates.ContainsKey(menuId))
                    submenuStates[menuId] = !submenuStates[menuId];
                else
                    submenuStates[menuId] = true;
            }
        }

        private bool IsSubmenuOpen(int menuId)
        {
            return submenuStates.TryGetValue(menuId, out var isOpen) && isOpen;
        }

        private async Task Navigate(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;
            
            NavigationManager.NavigateTo(path);
            if (IsMobileOpen)
            {
                await OnCloseMobileMenu.InvokeAsync();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;
            
            // Unsubscribe from event
            if (_authStateChangedHandler != null)
            {
                AuthService.OnAuthStateChanged -= _authStateChangedHandler;
            }
            
            // Dispose semaphore
            _loadMenusSemaphore?.Dispose();
        }
    }
}
