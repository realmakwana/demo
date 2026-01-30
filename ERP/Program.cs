using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using ERP.Models.DbContext;
using ERP.Models.Extensions;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Syncfusion Blazor service
builder.Services.AddSyncfusionBlazor();

// Add Entity Framework Core with SQL Server - Using Factory for Blazor Server
builder.Services.AddDbContextFactory<ERPDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register all DAL Services (Auth, Toast, Business Services)
builder.Services.AddDALServices();

// Register Syncfusion license (Community license - will be registered after obtaining)
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF5cXGpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1cc3RUQmdZWExwWUVWYEs=\r\n ");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
