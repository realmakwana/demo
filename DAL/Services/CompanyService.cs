using Microsoft.EntityFrameworkCore;
using TransportERP.Models.Entities;
using TransportERP.Models.DTOs;
using TransportERP.Models.DbContext;

namespace TransportERP.Models.Services;

public class CompanyService : ICompanyService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CompanyService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<CompanyDto>> GetAllCompaniesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var companies = await context.Companies
            .OrderByDescending(c => c.CompanyID)
            .ToListAsync();

        return companies.Select(MapToViewModel).ToList();
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(int companyId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var company = await context.Companies.FindAsync(companyId);
        return company != null ? MapToViewModel(company) : null;
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyDto CompanyDto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var company = MapToEntity(CompanyDto);
        company.CreatedDate = DateTime.Now;

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        CompanyDto.CompanyID = company.CompanyID;
        return CompanyDto;
    }

    public async Task<CompanyDto> UpdateCompanyAsync(CompanyDto CompanyDto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var company = await context.Companies.FindAsync(CompanyDto.CompanyID);
        if (company == null)
            throw new Exception($"Company with ID {CompanyDto.CompanyID} not found");

        // Update properties
        company.CompanyName = CompanyDto.CompanyName;
        company.ShortCode = CompanyDto.ShortCode;
        company.CompanyAddress = CompanyDto.CompanyAddress;
        company.GSTNO = CompanyDto.GSTNO;
        company.PANNO = CompanyDto.PANNO;
        company.PhoneNo = CompanyDto.PhoneNo;
        company.MobileNo = CompanyDto.MobileNo;
        company.Mail = CompanyDto.Mail;
        company.MailKey = CompanyDto.MailKey;
        company.StartDate = CompanyDto.StartDate;
        company.EndDate = CompanyDto.EndDate;
        company.IsActive = CompanyDto.IsActive;

        context.Companies.Update(company);
        await context.SaveChangesAsync();

        return CompanyDto;
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

    public async Task<List<CompanyDto>> GetActiveCompaniesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var companies = await context.Companies
            .Where(c => c.IsActive == true)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();

        return companies.Select(MapToViewModel).ToList();
    }

    // Mapping methods
    private CompanyDto MapToViewModel(Company company)
    {
        return new CompanyDto
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

    private Company MapToEntity(CompanyDto viewModel)
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
