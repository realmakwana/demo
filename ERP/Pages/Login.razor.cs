using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Inputs;
using TransportERP.Models.Services;
using System.ComponentModel.DataAnnotations;

namespace ERP.Pages
{
    public partial class Login : ComponentBase, IDisposable
    {
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private LoginModel loginModel = new();
        private string errorMessage = "";
        private bool isLoading = false;
        private bool showPassword = false;
        private InputType passwordInputType => showPassword ? InputType.Text : InputType.Password;

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }

        protected override void OnInitialized()
        {
            AuthService.OnAuthStateChanged += HandleAuthStateChanged;
        }

        private void HandleAuthStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            AuthService.OnAuthStateChanged -= HandleAuthStateChanged;
        }

        private async Task HandleLogin()
        {
            errorMessage = "";
            isLoading = true;

            await Task.Delay(800);

            if (await AuthService.Login(loginModel.Username, loginModel.Password))
            {
                await Task.Delay(300);
                Navigation.NavigateTo("/");
            }
            else
            {
                errorMessage = "Invalid credentials. Access denied.";
                isLoading = false;
            }
        }

        public class LoginModel
        {
            [Required(ErrorMessage = "Username is required")]
            public string Username { get; set; } = "";

            [Required(ErrorMessage = "Password is required")]
            [MinLength(3, ErrorMessage = "Password must be at least 3 characters")]
            public string Password { get; set; } = "";
        }
    }
}
