using Microsoft.EntityFrameworkCore;
using ERP.Models.Entities;
using ERP.Models.DbContext;

namespace ERP.Models.Services;

public interface ICompanyService
{
    Task<List<Company>> GetAllCompaniesAsync();
    Task<Company?> GetCompanyByIdAsync(int companyId);
    Task<Company> CreateCompanyAsync(Company company);
    Task<Company> UpdateCompanyAsync(Company company);
    Task<bool> DeleteCompanyAsync(int companyId);
    Task<int> GetTotalCompaniesCountAsync();
    Task<List<Company>> GetActiveCompaniesAsync();
}

public class CompanyService : ICompanyService
{
    private readonly IDbContextFactory<ERPDbContext> _contextFactory;

    public CompanyService(IDbContextFactory<ERPDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Companies
            .OrderByDescending(c => c.CompanyID)
            .ToListAsync();
    }

    public async Task<Company?> GetCompanyByIdAsync(int companyId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Companies.FindAsync(companyId);
    }

    public async Task<Company> CreateCompanyAsync(Company company)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        company.CreatedDate = DateTime.Now;

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        return company;
    }

    public async Task<Company> UpdateCompanyAsync(Company company)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        context.Companies.Update(company);
        await context.SaveChangesAsync();

        return company;
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

    public async Task<List<Company>> GetActiveCompaniesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Companies
            .Where(c => c.IsActive == true)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }
}
