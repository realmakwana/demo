using Microsoft.AspNetCore.Components;
using TransportERP.Models.DTOs;
using TransportERP.Models.Services;

namespace ERP.Pages
{
    public partial class Users : ComponentBase
    {
        [Inject] private IUserService UserService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<UserDto> users = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadUsers();
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
                users = new List<UserDto>();
            }
        }

        private async Task SaveUser(UserDto user)
        {
            try
            {
                if (user.Id == 0)
                {
                    await UserService.CreateUserAsync(user);
                    ToastService.ShowToast("User created successfully!", ToastLevel.Success);
                }
                else
                {
                    await UserService.UpdateUserAsync(user);
                    ToastService.ShowToast("User updated successfully!", ToastLevel.Success);
                }

                await LoadUsers();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving user: {ex.Message}", ToastLevel.Error);
            }
        }

        private async Task DeleteUser(UserDto user)
        {
            try
            {
                var success = await UserService.DeleteUserAsync(user.Id);
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
            var user = context as UserDto;
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
