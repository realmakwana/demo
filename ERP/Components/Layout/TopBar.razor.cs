using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using TransportERP.Models.Services;
using TransportERP.Models.Entities;

namespace ERP.Components.Layout
{
    public partial class TopBar : ComponentBase
    {
        [Parameter] public EventCallback OnToggleMobileMenu { get; set; }
        [Parameter] public EventCallback OnToggleSettings { get; set; }
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;

        private string searchQuery = string.Empty;
        private bool isSearchFocused = false;
        private string currentUser => AuthService.UserName ?? "Guest";
        private string currentRole = "Administrator";
        private bool showProfileMenu = false;
        private bool isDarkMode = false;
        private List<Menu> allMenus = new();
        private int selectedSearchIndex = -1;
        private ElementReference searchInputRef;

        protected override async Task OnInitializedAsync()
        {
            if (AuthService.IsAuthenticated)
            {
                allMenus = await MenuService.GetUserMenusAsync(AuthService.UserId);
            }
        }

        private class SearchMenuItem
        {
            public string Title { get; set; } = "";
            public string Url { get; set; } = "";
            public string Category { get; set; } = "";
            public string Icon { get; set; } = "";
        }

        private IEnumerable<SearchMenuItem> GetFilteredMenuItems()
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return Enumerable.Empty<SearchMenuItem>();
            
            var query = searchQuery.ToLower();
            
            // Search in all menus from database
            var results = allMenus
                .Where(m => !string.IsNullOrEmpty(m.MenuUrl)) // Only menus with URLs
                .Where(m => m.MenuDispName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                           m.MenuName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(m => new SearchMenuItem
                {
                    Title = m.MenuDispName,
                    Url = m.MenuUrl ?? "",
                    Category = GetMenuCategory(m.ParentMenuID),
                    Icon = GetMenuIcon(m.ClassName)
                })
                .Take(8); // Show more results
            
            return results;
        }

        private void HandleSearchKeyDown(KeyboardEventArgs e)
        {
            var results = GetFilteredMenuItems().ToList();
            if (!results.Any()) return;

            switch (e.Key)
            {
                case "ArrowDown":
                    // Move down
                    selectedSearchIndex = (selectedSearchIndex + 1) % results.Count;
                    break;

                case "ArrowUp":
                    // Move up
                    selectedSearchIndex = selectedSearchIndex <= 0 ? results.Count - 1 : selectedSearchIndex - 1;
                    break;

                case "Enter":
                    // Navigate to selected item
                    if (selectedSearchIndex >= 0 && selectedSearchIndex < results.Count)
                    {
                        NavigateToResult(results[selectedSearchIndex].Url);
                    }
                    break;

                case "Escape":
                    // Close search
                    searchQuery = "";
                    isSearchFocused = false;
                    selectedSearchIndex = -1;
                    break;
            }
        }

        private string GetMenuCategory(int? parentMenuId)
        {
            if (parentMenuId == null || parentMenuId == 0)
                return "Main";
            
            var parent = allMenus.FirstOrDefault(m => m.MenuID == parentMenuId);
            if (parent != null)
            {
                // If parent also has a parent, get grandparent
                if (parent.ParentMenuID != null && parent.ParentMenuID != 0)
                {
                    var grandParent = allMenus.FirstOrDefault(m => m.MenuID == parent.ParentMenuID);
                    return grandParent?.MenuDispName ?? parent.MenuDispName;
                }
                return parent.MenuDispName;
            }
            
            return "Other";
        }

        private string GetMenuIcon(string? className)
        {
            // Simple icon based on menu type
            return "<svg width='16' height='16' fill='none' stroke='currentColor' stroke-width='2' viewBox='0 0 24 24'><circle cx='12' cy='12' r='10'/></svg>";
        }

        private async Task HandleSearchBlur()
        {
            await Task.Delay(200);
            isSearchFocused = false;
            selectedSearchIndex = -1;
        }

        private void NavigateToResult(string url)
        {
            searchQuery = "";
            selectedSearchIndex = -1;
            Navigation.NavigateTo(url);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isDarkMode = await JSRuntime.InvokeAsync<bool>("isDarkMode");
                if (isDarkMode)
                {
                    await JSRuntime.InvokeVoidAsync("setTheme", "dark");
                }
                StateHasChanged();
            }
        }

        private async Task ToggleTheme(MouseEventArgs e)
        {
            isDarkMode = !isDarkMode;
            var theme = isDarkMode ? "dark" : "light";
            await JSRuntime.InvokeVoidAsync("toggleTheme", theme, e.ClientX, e.ClientY);
        }

        private string GetUserInitials()
        {
            if (string.IsNullOrEmpty(currentUser))
                return "G";

            var parts = currentUser.Split(' ');
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();

            return currentUser[0].ToString().ToUpper();
        }

        private void ToggleProfileMenu()
        {
            showProfileMenu = !showProfileMenu;
        }

        private void NavigateTo(string url)
        {
            showProfileMenu = false;
            Navigation.NavigateTo(url);
        }

        private async Task HandleLogout()
        {
            showProfileMenu = false;
            await AuthService.Logout();
            Navigation.NavigateTo("/login");
        }

        public async Task FocusSearch()
        {
            try
            {
                await searchInputRef.FocusAsync();
                isSearchFocused = true;
            }
            catch
            {
                // Ignore focus errors
            }
        }
    }
}
