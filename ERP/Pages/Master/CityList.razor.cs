using Microsoft.AspNetCore.Components;
using ERP.Models.Entities;
using ERP.Components.Shared.UI;
using ERP.Models.Services;

namespace ERP.Pages.Master
{
    public partial class CityList : ComponentBase
    {
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Master", Url = "#" },
            new() { Label = "Cities", Url = "/cities" }
        };

        private List<City> cities = new();
        private City? currentCity;
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        protected override async Task OnInitializedAsync()
        {
            LoadCities();
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

        private void LoadCities()
        {
            cities = new List<City>
            {
                new City { CityID = 1, CityName = "Mumbai", State = "Maharashtra", IsActive = true },
                new City { CityID = 2, CityName = "Delhi", State = "Delhi", IsActive = true },
                new City { CityID = 3, CityName = "Bangalore", State = "Karnataka", IsActive = false }
            };
        }

        private void OpenAddForm()
        {
            currentCity = new City
            {
                IsActive = true
            };
            showForm = true;
        }

        private void OpenEditForm(City city)
        {
            currentCity = new City
            {
                CityID = city.CityID,
                CityName = city.CityName,
                State = city.State,
                IsActive = city.IsActive
            };
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentCity = null;
        }

        private void SaveCity()
        {
            if (currentCity == null) return;

            if (currentCity.CityID == 0)
            {
                currentCity.CityID = cities.Any() ? cities.Max(c => c.CityID) + 1 : 1;
                cities.Add(currentCity);
                ToastService.ShowToast("City created successfully!", ToastLevel.Success);
            }
            else
            {
                var index = cities.FindIndex(c => c.CityID == currentCity.CityID);
                if (index >= 0)
                {
                    cities[index] = currentCity;
                    ToastService.ShowToast("City updated successfully!", ToastLevel.Success);
                }
            }
            
            CloseForm();
        }

        private void DeleteCity(City city)
        {
            cities.Remove(city);
            ToastService.ShowToast("City deleted successfully!", ToastLevel.Success);
        }
    }
}
