using Microsoft.AspNetCore.Components;
using ERP.Models.Services;
using ERP.Components.Shared.UI;
using UserEntity = ERP.Models.Entities.User;
using UserCategoryEntity = ERP.Models.Entities.UserCategory;
using ERP.Models.Entities;

namespace ERP.Pages.Master
{
    public partial class Users : ComponentBase
    {
        [Inject] private IUserService UserService { get; set; } = default!;
        [Inject] private IUserCategoryService UserCategoryService { get; set; } = default!;
        [Inject] private ICompanyService CompanyService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Master", Url = "#" },
            new() { Label = "Users", Url = "/users" }
        };

        private List<UserEntity> users = new();
        private List<UserCategoryEntity> userCategories = new();
        private List<ERP.Models.Entities.Company> companies = new();
        private UserEntity? currentUser;
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        protected override async Task OnInitializedAsync()
        {
            await LoadUsers();
            await LoadDropdownData();
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

        private async Task LoadUsers()
        {
            try
            {
                users = await UserService.GetAllUsersAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading users: {ex.Message}", ToastLevel.Error);
                users = new List<UserEntity>();
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Load user categories from service
                userCategories = await UserCategoryService.GetAllUserCategoriesAsync();
                
                // Load companies from service
                companies = await CompanyService.GetAllCompaniesAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading dropdown data: {ex.Message}", ToastLevel.Error);
                userCategories = new List<UserCategoryEntity>();
                companies = new List<Company>();
            }
        }

        private void OpenAddForm()
        {
            currentUser = new UserEntity
            {
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            showForm = true;
        }

        private void OpenEditForm(UserEntity user)
        {
            currentUser = new UserEntity
            {
                UserID = user.UserID,
                UserName = user.UserName,
                EmailID = user.EmailID,
                Password = user.Password,
                MobileNo = user.MobileNo,
                UserCategoryID = user.UserCategoryID,
                CompanyID = user.CompanyID,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentUser = null;
        }

        private async Task SaveUser()
        {
            if (currentUser == null) return;

            try
            {
                if (currentUser.UserID == 0)
                {
                    await UserService.CreateUserAsync(currentUser);
                    ToastService.ShowToast("User created successfully!", ToastLevel.Success);
                }
                else
                {
                    await UserService.UpdateUserAsync(currentUser);
                    ToastService.ShowToast("User updated successfully!", ToastLevel.Success);
                }

                await LoadUsers();
                CloseForm();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving user: {ex.Message}", ToastLevel.Error);
            }
        }

        private async Task DeleteUser(UserEntity user)
        {
            try
            {
                var success = await UserService.DeleteUserAsync(user.UserID);
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
    }
}
