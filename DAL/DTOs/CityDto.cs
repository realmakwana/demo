using System;
using System.ComponentModel.DataAnnotations;
using TransportERP.Models.Attributes;

namespace TransportERP.Models.DTOs
{
    public class CityDto
    {
        [CrudField(Label = "ID", Order = 1, ShowInGrid = true, GridWidth = "80", HideInForm = true)]
        public int Id { get; set; }

        [CrudField(Label = "City Name", Order = 2, Required = true, FieldType = "Text", ShowInGrid = true, GridWidth = "200")]
        [Required(ErrorMessage = "City Name is required")]
        public string CityName { get; set; } = string.Empty;

        [CrudField(Label = "State", Order = 3, FieldType = "Text", ShowInGrid = true, GridWidth = "150")]
        public string State { get; set; } = string.Empty;

        [CrudField(Label = "Active", Order = 4, FieldType = "Checkbox", ShowInGrid = true, GridWidth = "100")]
        public bool IsActive { get; set; } = true;
    }
}
