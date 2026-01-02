using Microsoft.Extensions.DependencyInjection;
using ERP.Models.Services;

namespace ERP.Models.Extensions;

/// <summary>
/// Extension methods for registering DAL services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all DAL services (Auth, Toast, and business services)
    /// </summary>
    public static IServiceCollection AddDALServices(this IServiceCollection services)
    {
        // Core Services
        services.AddScoped<ToastService>();
        services.AddScoped<AuthService>();

        // Business Services
        services.AddScoped<IUserCategoryService, UserCategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IMenuService, MenuService>();

        return services;
    }
}
