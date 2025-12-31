using Microsoft.AspNetCore.Components;
using TransportERP.Models.DTOs;

namespace ERP.Pages
{
    public partial class Vehicles : ComponentBase
    {
        private List<VehicleDto> vehicles = new();

        protected override void OnInitialized()
        {
            LoadVehicles();
        }

        private void LoadVehicles()
        {
            vehicles = new List<VehicleDto>
            {
                new VehicleDto 
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
                new VehicleDto 
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

        private void SaveVehicle(VehicleDto vehicle)
        {
            if (vehicle.Id == 0)
            {
                vehicle.Id = vehicles.Any() ? vehicles.Max(v => v.Id) + 1 : 1;
                vehicle.CreatedDate = DateTime.Now;
                vehicle.CreatedBy = "Admin";
                vehicles.Add(vehicle);
            }
            else
            {
                var index = vehicles.FindIndex(v => v.Id == vehicle.Id);
                if (index >= 0)
                {
                    vehicles[index] = vehicle;
                }
            }
        }

        private void DeleteVehicle(VehicleDto vehicle)
        {
            vehicles.Remove(vehicle);
        }

        private RenderFragment RenderGridTemplate(string fieldName, object context) => builder =>
        {
            var vehicle = context as VehicleDto;
            
            if (fieldName == "ActiveBadge" && vehicle != null)
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", vehicle.IsActive ? "badge-success" : "badge-danger");
                builder.AddContent(2, vehicle.IsActive ? "Active" : "Inactive");
                builder.CloseElement();
            }
        };
    }
}
