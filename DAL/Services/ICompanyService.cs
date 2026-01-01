using TransportERP.Models.DTOs;

namespace TransportERP.Models.Services;

public interface ICompanyService
{
    Task<List<CompanyDto>> GetAllCompaniesAsync();
    Task<CompanyDto?> GetCompanyByIdAsync(int companyId);
    Task<CompanyDto> CreateCompanyAsync(CompanyDto company);
    Task<CompanyDto> UpdateCompanyAsync(CompanyDto company);
    Task<bool> DeleteCompanyAsync(int companyId);
    Task<int> GetTotalCompaniesCountAsync();
    Task<List<CompanyDto>> GetActiveCompaniesAsync();
}
