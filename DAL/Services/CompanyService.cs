using Microsoft.EntityFrameworkCore;
using TransportERP.Models.Entities;
using TransportERP.Models.ViewModels;
using TransportERP.Models.DbContext;

namespace TransportERP.Models.Services;

public class CompanyService : ICompanyService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CompanyService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<CompanyViewModel>> GetAllCompaniesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var companies = await context.Companies
            .OrderByDescending(c => c.CompanyID)
            .ToListAsync();

        return companies.Select(MapToViewModel).ToList();
    }

    public async Task<CompanyViewModel?> GetCompanyByIdAsync(int companyId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var company = await context.Companies.FindAsync(companyId);
        return company != null ? MapToViewModel(company) : null;
    }

    public async Task<CompanyViewModel> CreateCompanyAsync(CompanyViewModel companyViewModel)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var company = MapToEntity(companyViewModel);
        company.CreatedDate = DateTime.Now;

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        companyViewModel.CompanyID = company.CompanyID;
        return companyViewModel;
    }

    public async Task<CompanyViewModel> UpdateCompanyAsync(CompanyViewModel companyViewModel)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var company = await context.Companies.FindAsync(companyViewModel.CompanyID);
        if (company == null)
            throw new Exception($"Company with ID {companyViewModel.CompanyID} not found");

        // Update properties
        company.CompanyName = companyViewModel.CompanyName;
        company.ShortCode = companyViewModel.ShortCode;
        company.CompanyAddress = companyViewModel.CompanyAddress;
        company.GSTNO = companyViewModel.GSTNO;
        company.PANNO = companyViewModel.PANNO;
        company.PhoneNo = companyViewModel.PhoneNo;
        company.MobileNo = companyViewModel.MobileNo;
        company.Mail = companyViewModel.Mail;
        company.MailKey = companyViewModel.MailKey;
        company.StartDate = companyViewModel.StartDate;
        company.EndDate = companyViewModel.EndDate;
        company.IsActive = companyViewModel.IsActive;

        context.Companies.Update(company);
        await context.SaveChangesAsync();

        return companyViewModel;
    }

    public async Task<bool> DeleteCompanyAsync(int companyId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var company = await context.Companies.FindAsync(companyId);
        if (company == null)
            return false;

        context.Companies.Remove(company);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetTotalCompaniesCountAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Companies.CountAsync();
    }

    public async Task<List<CompanyViewModel>> GetActiveCompaniesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var companies = await context.Companies
            .Where(c => c.IsActive == true)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();

        return companies.Select(MapToViewModel).ToList();
    }

    // Mapping methods
    private CompanyViewModel MapToViewModel(Company company)
    {
        return new CompanyViewModel
        {
            CompanyID = company.CompanyID,
            CompanyName = company.CompanyName ?? string.Empty,
            ShortCode = company.ShortCode,
            CompanyAddress = company.CompanyAddress,
            GSTNO = company.GSTNO,
            PANNO = company.PANNO,
            PhoneNo = company.PhoneNo,
            MobileNo = company.MobileNo,
            Mail = company.Mail,
            MailKey = company.MailKey,
            StartDate = company.StartDate,
            EndDate = company.EndDate,
            CreatedDate = company.CreatedDate,
            IsActive = company.IsActive ?? true
        };
    }

    private Company MapToEntity(CompanyViewModel viewModel)
    {
        return new Company
        {
            CompanyID = viewModel.CompanyID,
            CompanyName = viewModel.CompanyName,
            ShortCode = viewModel.ShortCode,
            CompanyAddress = viewModel.CompanyAddress,
            GSTNO = viewModel.GSTNO,
            PANNO = viewModel.PANNO,
            PhoneNo = viewModel.PhoneNo,
            MobileNo = viewModel.MobileNo,
            Mail = viewModel.Mail,
            MailKey = viewModel.MailKey,
            StartDate = viewModel.StartDate,
            EndDate = viewModel.EndDate,
            CreatedDate = viewModel.CreatedDate,
            IsActive = viewModel.IsActive
        };
    }
}
