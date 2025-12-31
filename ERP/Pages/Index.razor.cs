using Microsoft.AspNetCore.Components;
using ERP.Components.Shared.UI;
using TransportERP.Models.Services;

namespace ERP.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IUserService UserService { get; set; } = default!;
        [Inject] private ICompanyService CompanyService { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new();

        private int totalUsers = 0;
        private int totalVehicles = 0;
        private int totalBookings = 0;
        private decimal totalRevenue = 0;

        protected override async Task OnInitializedAsync()
        {
            breadcrumbItems = new List<AppBreadcrumbItem>
            {
                new AppBreadcrumbItem("Home", "/"),
                new AppBreadcrumbItem("Dashboard", null)
            };

            await LoadStats();
        }

        private async Task LoadStats()
        {
            try
            {
                totalUsers = await UserService.GetTotalUsersCountAsync();
                
                // For now, these are still placeholders as we focus on Users and Rights
                // But we set them up to be dynamic later
                totalVehicles = 0; 
                totalBookings = 0;
                totalRevenue = 0;
            }
            catch (Exception)
            {
                // Fallback or log error
            }
        }

        private void NavigateTo(string path)
        {
            NavigationManager.NavigateTo(path);
        }
    }
}
