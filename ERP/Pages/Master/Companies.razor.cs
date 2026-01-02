using Microsoft.AspNetCore.Components;
using ERP.Models.Entities;
using ERP.Models.Services;
using ERP.Components.Shared.UI;

namespace ERP.Pages.Master
{
    public partial class Companies : ComponentBase
    {
        [Inject] private ICompanyService CompanyService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Master", Url = "#" },
            new() { Label = "Companies", Url = "/companies" }
        };

        private List<Company> companies = new();
        private Company? currentCompany;
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        protected override async Task OnInitializedAsync()
        {
            await LoadCompanies();
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

        private async Task LoadCompanies()
        {
            try
            {
                companies = await CompanyService.GetAllCompaniesAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading companies: {ex.Message}", ToastLevel.Error);
                companies = new List<Company>();
            }
        }

        private void OpenAddForm()
        {
            currentCompany = new Company
            {
                IsActive = true,
                CreatedDate = DateTime.Now,
                StartDate = DateTime.Now
            };
            showForm = true;
        }

        private void OpenEditForm(Company company)
        {
            currentCompany = new Company
            {
                CompanyID = company.CompanyID,
                CompanyName = company.CompanyName,
                CompanyAddress = company.CompanyAddress,
                GSTNO = company.GSTNO,
                PANNO = company.PANNO,
                IsActive = company.IsActive,
                PhoneNo = company.PhoneNo,
                MobileNo = company.MobileNo,
                StartDate = company.StartDate,
                EndDate = company.EndDate,
                ShortCode = company.ShortCode,
                Mail = company.Mail,
                MailKey = company.MailKey,
                CreatedDate = company.CreatedDate
            };
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentCompany = null;
        }

        private async Task SaveCompany()
        {
            if (currentCompany == null) return;

            try
            {
                if (currentCompany.CompanyID == 0)
                {
                    await CompanyService.CreateCompanyAsync(currentCompany);
                    ToastService.ShowToast("Company created successfully!", ToastLevel.Success);
                }
                else
                {
                    await CompanyService.UpdateCompanyAsync(currentCompany);
                    ToastService.ShowToast("Company updated successfully!", ToastLevel.Success);
                }

                await LoadCompanies();
                CloseForm();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving company: {ex.Message}", ToastLevel.Error);
            }
        }

        private async Task DeleteCompany(Company company)
        {
            try
            {
                var success = await CompanyService.DeleteCompanyAsync(company.CompanyID);
                if (success)
                {
                    ToastService.ShowToast("Company deleted successfully!", ToastLevel.Success);
                    await LoadCompanies();
                }
                else
                {
                    ToastService.ShowToast("Company not found!", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error deleting company: {ex.Message}", ToastLevel.Error);
            }
        }
    }
}
