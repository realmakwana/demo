using Microsoft.AspNetCore.Components;
using TransportERP.Models.ViewModels;
using TransportERP.Models.Services;

namespace ERP.Pages
{
    public partial class UserCategory
    {
        [Inject] private IUserCategoryService UserCategoryService{ get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<UserCategoryViewModel> users = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                users = await UserCategoryService.GetAllUserCategoriesAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading users: {ex.Message}", ToastLevel.Error);
                users = new List<UserCategoryViewModel>();
            }
        }

        private async Task SaveUser(UserCategoryViewModel user)
        {
            try
            {
                if (user.UserCategoryID == 0)
                {
                    await UserCategoryService.CreateUserCategoryAsync(user);
                    ToastService.ShowToast("User created successfully!", ToastLevel.Success);
                }
                else
                {
                    await UserCategoryService.UpdateUserCategoryAsync(user);
                    ToastService.ShowToast("User updated successfully!", ToastLevel.Success);
                }

                await LoadUsers();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving user: {ex.Message}", ToastLevel.Error);
            }
        }

        private async Task DeleteUser(UserCategoryViewModel user)
        {
            try
            {
                var success = await UserCategoryService.DeleteUserCategoryAsync(user.UserCategoryID);
                if (success)
                {
                    ToastService.ShowToast("User deleted successfully!", ToastLevel.Success);
                    await LoadUsers();
                }
                else
                {
                    ToastService.ShowToast("User not found!", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error deleting user: {ex.Message}", ToastLevel.Error);
            }
        }

        private RenderFragment RenderGridTemplate(string templateName, object context) => builder =>
        {
            var user = context as UserCategoryViewModel;
            if (templateName == "StatusBadge" && user != null)
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", user.IsActive ? "badge-success" : "badge-danger");
                builder.AddContent(2, user.IsActive ? "Active" : "Inactive");
                builder.CloseElement();
            }
        };
    }
}
