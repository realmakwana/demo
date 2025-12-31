using Microsoft.AspNetCore.Components;
using TransportERP.Models.DTOs;

namespace ERP.Pages
{
    public partial class CityList : ComponentBase
    {
        private List<CityDto> cities = new();

        protected override void OnInitialized()
        {
            cities = new List<CityDto>
            {
                new CityDto { Id = 1, CityName = "Mumbai", State = "Maharashtra", IsActive = true },
                new CityDto { Id = 2, CityName = "Delhi", State = "Delhi", IsActive = true },
                new CityDto { Id = 3, CityName = "Bangalore", State = "Karnataka", IsActive = false }
            };
        }

        private void SaveCity(CityDto city)
        {
            if (city.Id == 0)
            {
                city.Id = cities.Any() ? cities.Max(c => c.Id) + 1 : 1;
                cities.Add(city);
            }
            else
            {
                var index = cities.FindIndex(c => c.Id == city.Id);
                if (index >= 0)
                {
                    cities[index] = city;
                }
            }
        }

        private void DeleteCity(CityDto city)
        {
            cities.Remove(city);
        }
    }
}
