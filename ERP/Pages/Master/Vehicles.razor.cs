using Microsoft.AspNetCore.Components;
using ERP.Models.Entities;
using ERP.Components.Shared.UI;
using ERP.Models.Services;

namespace ERP.Pages.Master
{
    public partial class Vehicles : ComponentBase
    {
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Master", Url = "#" },
            new() { Label = "Vehicles", Url = "/vehicles" }
        };

        private List<Vehicle> vehicles = new();
        private Vehicle? currentVehicle;
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        protected override async Task OnInitializedAsync()
        {
            LoadVehicles();
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

        private void LoadVehicles()
        {
            vehicles = new List<Vehicle>
            {
                new Vehicle 
                { 
                    Id = 1, 
                    VehicleNumber = "MH12AB1234", 
                    VehicleType = "Truck", 
                    Capacity = 10.5m,
                    ModelYear = 2020,
                    InsuranceExpiry = DateTime.Now.AddMonths(6),
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "Admin"
                },
                new Vehicle 
                { 
                    Id = 2, 
                    VehicleNumber = "MH12CD5678", 
                    VehicleType = "Trailer", 
                    Capacity = 15.0m,
                    ModelYear = 2021,
                    InsuranceExpiry = DateTime.Now.AddMonths(3),
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "Admin"
                }
            };
        }

        private void OpenAddForm()
        {
            currentVehicle = new Vehicle
            {
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            showForm = true;
        }

        private void OpenEditForm(Vehicle vehicle)
        {
            currentVehicle = new Vehicle
            {
                Id = vehicle.Id,
                VehicleNumber = vehicle.VehicleNumber,
                VehicleType = vehicle.VehicleType,
                Capacity = vehicle.Capacity,
                ModelYear = vehicle.ModelYear,
                InsuranceExpiry = vehicle.InsuranceExpiry,
                IsActive = vehicle.IsActive,
                CreatedDate = vehicle.CreatedDate,
                CreatedBy = vehicle.CreatedBy
            };
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentVehicle = null;
        }

        private void SaveVehicle()
        {
            if (currentVehicle == null) return;

            if (currentVehicle.Id == 0)
            {
                currentVehicle.Id = vehicles.Any() ? vehicles.Max(v => v.Id) + 1 : 1;
                currentVehicle.CreatedDate = DateTime.Now;
                currentVehicle.CreatedBy = "Admin";
                vehicles.Add(currentVehicle);
                ToastService.ShowToast("Vehicle created successfully!", ToastLevel.Success);
            }
            else
            {
                var index = vehicles.FindIndex(v => v.Id == currentVehicle.Id);
                if (index >= 0)
                {
                    vehicles[index] = currentVehicle;
                    ToastService.ShowToast("Vehicle updated successfully!", ToastLevel.Success);
                }
            }
            
            CloseForm();
        }

        private void DeleteVehicle(Vehicle vehicle)
        {
            vehicles.Remove(vehicle);
            ToastService.ShowToast("Vehicle deleted successfully!", ToastLevel.Success);
        }
    }
}
