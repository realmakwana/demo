using Microsoft.AspNetCore.Components;
using ERP.Models.Services;
using ERP.Components.Shared.UI;
using UserCategoryEntity = ERP.Models.Entities.UserCategory;
using ERP.Models.Entities;

namespace ERP.Pages.Master
{
    public partial class UserCategory : ComponentBase
    {
        [Inject] private IUserCategoryService UserCategoryService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Master", Url = "#" },
            new() { Label = "User Categories", Url = "/usercategory" }
        };

        private List<UserCategoryEntity> categories = new();
        private UserCategoryEntity? currentCategory;
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        protected override async Task OnInitializedAsync()
        {
            await LoadCategories();
            await LoadRights();
        }

        private async Task LoadRights()
        {
            if (AuthService.IsAuthenticated)
            {
                var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
                var path = "/" + relativePath.Split('?')[0];
                currentRights = await MenuService.GetUserMenuRightsAsync(AuthService.UserId, path);
            }
        }

        private async Task LoadCategories()
        {
            try
            {
                categories = await UserCategoryService.GetAllUserCategoriesAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading user categories: {ex.Message}", ToastLevel.Error);
                categories = new List<UserCategoryEntity>();
            }
        }

        private void OpenAddForm()
        {
            currentCategory = new UserCategoryEntity
            {
                IsActive = true
            };
            showForm = true;
        }

        private void OpenEditForm(UserCategoryEntity category)
        {
            currentCategory = new UserCategoryEntity
            {
                UserCategoryID = category.UserCategoryID,
                UserCategoryName = category.UserCategoryName,
                IsActive = category.IsActive
            };
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentCategory = null;
        }

        private async Task SaveCategory()
        {
            if (currentCategory == null) return;

            try
            {
                if (currentCategory.UserCategoryID == 0)
                {
                    await UserCategoryService.CreateUserCategoryAsync(currentCategory);
                    ToastService.ShowToast("User category created successfully!", ToastLevel.Success);
                }
                else
                {
                    await UserCategoryService.UpdateUserCategoryAsync(currentCategory);
                    ToastService.ShowToast("User category updated successfully!", ToastLevel.Success);
                }

                await LoadCategories();
                CloseForm();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving user category: {ex.Message}", ToastLevel.Error);
            }
        }

        private async Task DeleteCategory(UserCategoryEntity category)
        {
            try
            {
                var success = await UserCategoryService.DeleteUserCategoryAsync(category.UserCategoryID);
                if (success)
                {
                    ToastService.ShowToast("User category deleted successfully!", ToastLevel.Success);
                    await LoadCategories();
                }
                else
                {
                    ToastService.ShowToast("User category not found!", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error deleting user category: {ex.Message}", ToastLevel.Error);
            }
        }
    }
}
