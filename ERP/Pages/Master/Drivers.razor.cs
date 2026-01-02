using ERP.Components.Shared.UI;
using ERP.Models.Entities;
using ERP.Models.Services;
using Microsoft.AspNetCore.Components;

namespace ERP.Pages.Master
{
    public partial class Drivers : ComponentBase
    {
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Master", Url = "#" },
            new() { Label = "Drivers", Url = "/drivers" }
        };

        private List<Driver> drivers = new();
        private Driver? currentDriver;
        private Driver? selectedDriverForView;
        private bool showForm = false;
        private bool showViewModal = false;
        private UserWiseMenu? currentRights;

        protected override async Task OnInitializedAsync()
        {
            LoadDrivers();
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

        private void LoadDrivers()
        {
            drivers = new List<Driver>();

            string[] firstNames = { "Suresh", "Ramesh", "Mahesh", "Rajesh", "Dinesh", "Mukesh", "Naresh", "Rakesh", "Umesh", "Ganesh" };
            string[] lastNames = { "Kumar", "Singh", "Yadav", "Sharma", "Verma", "Gupta", "Patel", "Reddy", "Nair", "Joshi" };
            string[] cities = { "DL", "UP", "MH", "GJ", "RJ", "PB", "HR", "KA", "TN", "AP" };

            for (int i = 1; i <= 50; i++)
            {
                var firstName = firstNames[i % firstNames.Length];
                var lastName = lastNames[(i / 2) % lastNames.Length];
                var city = cities[i % cities.Length];

                drivers.Add(new Driver
                {
                    Id = i,
                    Name = $"{firstName} {lastName} {i}",
                    Mobile = $"{9800000000 + i}",
                    LicenseNumber = $"{city}-{(i % 5) + 1}{2020 + (i % 5)}{1000000 + i}",
                    CreatedDate = DateTime.Now.AddDays(-i),
                    IsActive = true
                });
            }
        }

        private void OpenAddForm()
        {
            currentDriver = new Driver
            {
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            showForm = true;
        }

        private void OpenEditForm(Driver driver)
        {
            currentDriver = new Driver
            {
                Id = driver.Id,
                Name = driver.Name,
                Mobile = driver.Mobile,
                LicenseNumber = driver.LicenseNumber,
                CreatedDate = driver.CreatedDate,
                IsActive = driver.IsActive
            };
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentDriver = null;
        }

        private void SaveDriver()
        {
            if (currentDriver == null) return;

            if (currentDriver.Id == 0)
            {
                currentDriver.Id = drivers.Any() ? drivers.Max(d => d.Id) + 1 : 1;
                drivers.Add(currentDriver);
                ToastService.ShowToast("Driver created successfully!", ToastLevel.Success);
            }
            else
            {
                var index = drivers.FindIndex(d => d.Id == currentDriver.Id);
                if (index >= 0)
                {
                    drivers[index] = currentDriver;
                    ToastService.ShowToast("Driver updated successfully!", ToastLevel.Success);
                }
            }

            CloseForm();
        }

        private void DeleteDriver(Driver driver)
        {
            drivers.Remove(driver);
            ToastService.ShowToast("Driver deleted successfully!", ToastLevel.Success);
        }

        private void ViewDriver(Driver driver)
        {
            selectedDriverForView = driver;
            showViewModal = true;
        }

        private void CloseViewModal()
        {
            showViewModal = false;
            selectedDriverForView = null;
        }
    }
}
