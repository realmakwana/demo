using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using TransportERP.Models.DbContext;
using System;
using System.Threading.Tasks;

namespace TransportERP.Models.Services
{
    public class AuthService
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private bool _isAuthenticated;
        private string? _userName;
        private int _userId;

        public AuthService(ProtectedLocalStorage localStorage, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _localStorage = localStorage;
            _contextFactory = contextFactory;
        }

        public bool IsAuthenticated 
        { 
            get => _isAuthenticated;
            private set => _isAuthenticated = value;
        }
        
        public string? UserName 
        { 
            get => _userName;
            private set => _userName = value;
        }

        public int UserId
        {
            get => _userId;
            private set => _userId = value;
        }

        public event Action? OnAuthStateChanged;

        public async Task<bool> CheckAuthStatus()
        {
            try
            {
                var userResult = await _localStorage.GetAsync<string>("userName");
                var idResult = await _localStorage.GetAsync<int>("userId");

                if (userResult.Success && !string.IsNullOrEmpty(userResult.Value))
                {
                    _isAuthenticated = true;
                    _userName = userResult.Value;
                    _userId = idResult.Success ? idResult.Value : 0;
                    NotifyStateChanged();
                    return true;
                }
            }
            catch (Exception)
            {
                // Local storage might not be available during pre-rendering
            }
            return false;
        }

        public async Task<bool> Login(string username, string password)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password && u.IsActive == true);

            if (user != null)
            {
                IsAuthenticated = true;
                UserName = user.UserName;
                UserId = user.UserID;
                await _localStorage.SetAsync("userName", user.UserName ?? "");
                await _localStorage.SetAsync("userId", user.UserID);
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        public async Task Logout()
        {
            IsAuthenticated = false;
            UserName = null;
            UserId = 0;
            await _localStorage.DeleteAsync("userName");
            await _localStorage.DeleteAsync("userId");
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnAuthStateChanged?.Invoke();
    }
}
