using Microsoft.AspNetCore.Components;
using TransportERP.Models.Services;
using ERP.Components.Shared.UI;
using TransportERP.Models.ViewModels;

namespace ERP.Pages
{
    public partial class UserProfile : ComponentBase
    {
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private ProfileViewModel profileModel = new();
        
        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new AppBreadcrumbItem("Home", "/"),
            new AppBreadcrumbItem("My Profile", null)
        };

        protected override void OnInitialized()
        {
            profileModel = new ProfileViewModel
            {
                FullName = AuthService.UserName ?? "Admin User",
                Email = "admin@transporterp.com",
                Mobile = "+91 99887 76655",
                Role = "System Administrator",
                Bio = "Overseeing fleet operations and logistics management."
            };
        }

        private void HandleSave()
        {
            ToastService.ShowToast("Profile updated successfully", ToastLevel.Success);
        }

        private string GetInitials()
        {
            var name = profileModel.FullName;
            var parts = name.Split(' ');
            if (parts.Length >= 2) return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return name.Substring(0, 1).ToUpper();
        }
    }
}
