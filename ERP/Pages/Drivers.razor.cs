using Microsoft.AspNetCore.Components;
using TransportERP.Models.DTOs;
using Syncfusion.Blazor.Buttons;
using ERP.Components.Shared.CRUD;

namespace ERP.Pages
{
    public partial class Drivers : ComponentBase
    {
        private List<DriverDto> drivers = new();
        private MasterPage<DriverDto> masterPage = default!;

        protected override void OnInitialized()
        {
            LoadDrivers();
        }

        private void LoadDrivers()
        {
            // Generate 50 dummy driver records for testing export functionality
            drivers = new List<DriverDto>();
            
            string[] firstNames = { "Suresh", "Ramesh", "Mahesh", "Rajesh", "Dinesh", "Mukesh", "Naresh", "Rakesh", "Umesh", "Ganesh" };
            string[] lastNames = { "Kumar", "Singh", "Yadav", "Sharma", "Verma", "Gupta", "Patel", "Reddy", "Nair", "Joshi" };
            string[] cities = { "DL", "UP", "MH", "GJ", "RJ", "PB", "HR", "KA", "TN", "AP" };
            
            for (int i = 1; i <= 50; i++)
            {
                var firstName = firstNames[i % firstNames.Length];
                var lastName = lastNames[(i / 2) % lastNames.Length];
                var city = cities[i % cities.Length];
                
                drivers.Add(new DriverDto
                {
                    Id = i,
                    Name = $"{firstName} {lastName} {i}",
                    Mobile = $"{9800000000 + i}",
                    LicenseNumber = $"{city}-{(i % 5) + 1}{2020 + (i % 5)}{1000000 + i}",
                    CreatedDate = DateTime.Now.AddDays(-i)
                });
            }
        }

        private void SaveDriver(DriverDto driver)
        {
            if (driver.Id == 0)
            {
                driver.Id = drivers.Any() ? drivers.Max(d => d.Id) + 1 : 1;
                drivers.Add(driver);
            }
            else
            {
                var index = drivers.FindIndex(d => d.Id == driver.Id);
                if (index >= 0) drivers[index] = driver;
            }
        }

        private void DeleteDriver(DriverDto driver) => drivers.Remove(driver);

        private void ViewDriver(DriverDto driver)
        {
            masterPage.OpenModal(driver);
        }

        private RenderFragment<DriverDto> CustomActionsTemplate => driver => builder =>
        {
            builder.OpenComponent<SfButton>(0);
            builder.AddAttribute(1, "CssClass", "action-icon-btn view-btn");
            builder.AddAttribute(2, "IconCss", "bi bi-eye");
            builder.AddAttribute(3, "IsPrimary", false);
            builder.AddAttribute(4, "title", "View Details");
            builder.AddAttribute(5, "OnClick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, (args) => ViewDriver(driver)));
            builder.CloseComponent();
        };
    }
}
