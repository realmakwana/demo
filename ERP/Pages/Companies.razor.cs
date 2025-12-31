using Microsoft.AspNetCore.Components;
using TransportERP.Models.ViewModels;
using TransportERP.Models.Services;

namespace ERP.Pages
{
    public partial class Companies : ComponentBase
    {
        [Inject] private ICompanyService CompanyService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<CompanyViewModel> companies = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadCompanies();
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
                companies = new List<CompanyViewModel>();
            }
        }

        private async Task SaveCompany(CompanyViewModel company)
        {
            try
            {
                if (company.CompanyID == 0)
                {
                    await CompanyService.CreateCompanyAsync(company);
                    ToastService.ShowToast("Company created successfully!", ToastLevel.Success);
                }
                else
                {
                    await CompanyService.UpdateCompanyAsync(company);
                    ToastService.ShowToast("Company updated successfully!", ToastLevel.Success);
                }

                await LoadCompanies();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving company: {ex.Message}", ToastLevel.Error);
            }
        }

        private async Task DeleteCompany(CompanyViewModel company)
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

        private RenderFragment RenderGridTemplate(string templateName, object context) => builder =>
        {
            var company = context as CompanyViewModel;
            if (templateName == "StatusBadge" && company != null)
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", company.IsActive ? "badge-success" : "badge-danger");
                builder.AddContent(2, company.IsActive ? "Active" : "Inactive");
                builder.CloseElement();
            }
        };
    }
}
