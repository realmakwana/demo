using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace ERP.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private bool isCollapsed = false;
        private bool isMobileMenuOpen = false;
        private bool isNavigating = false;
        private bool isSettingsOpen = false;
        private TopBar? topBarRef;

        protected override void OnInitialized()
        {
            Navigation.LocationChanged += HandleLocationChanged;
        }

        private async void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            isNavigating = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(600);
            isNavigating = false;
            await InvokeAsync(StateHasChanged);
        }

        private void HandleGlobalKeyDown(KeyboardEventArgs e)
       {
            // Check if / key is pressed and not in an input field
            if (e.Key == "/" && topBarRef != null)
            {
                // Focus the search box
                topBarRef.FocusSearch();
            }
        }

        private void ToggleMobileMenu()
        {
            isMobileMenuOpen = !isMobileMenuOpen;
        }

        private void ToggleSidebar()
        {
            isCollapsed = !isCollapsed;
        }

        private void CloseMobileMenu()
        {
            isMobileMenuOpen = false;
        }

        private void ToggleSettings()
        {
            isSettingsOpen = !isSettingsOpen;
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= HandleLocationChanged;
        }
    }
}
