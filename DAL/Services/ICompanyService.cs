using TransportERP.Models.ViewModels;

namespace TransportERP.Models.Services;

public interface ICompanyService
{
    Task<List<CompanyViewModel>> GetAllCompaniesAsync();
    Task<CompanyViewModel?> GetCompanyByIdAsync(int companyId);
    Task<CompanyViewModel> CreateCompanyAsync(CompanyViewModel company);
    Task<CompanyViewModel> UpdateCompanyAsync(CompanyViewModel company);
    Task<bool> DeleteCompanyAsync(int companyId);
    Task<int> GetTotalCompaniesCountAsync();
    Task<List<CompanyViewModel>> GetActiveCompaniesAsync();
}
