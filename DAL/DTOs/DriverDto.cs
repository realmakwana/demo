using System;
using System.ComponentModel.DataAnnotations;
using TransportERP.Models.Attributes;

namespace TransportERP.Models.DTOs
{
    public class DriverDto
    {
        [CrudField(Label = "ID", Order = 1, ShowInGrid = true, GridWidth = "80", HideInForm = true)]
        public int Id { get; set; }

        [CrudField(Label = "Name", Order = 2, Required = true, FieldType = "Text", ShowInGrid = true, GridWidth = "200")]
        [Required(ErrorMessage = "Driver Name is required")]
        public string Name { get; set; } = string.Empty;

        [CrudField(Label = "Mobile", Order = 3, Required = true, FieldType = "Text", ShowInGrid = true, GridWidth = "150")]
        [Required(ErrorMessage = "Mobile Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Mobile { get; set; } = string.Empty;

        [CrudField(Label = "License Number", Order = 4, Required = true, FieldType = "Text", ShowInGrid = true, GridWidth = "200")]
        [Required(ErrorMessage = "License Number is required")]
        public string LicenseNumber { get; set; } = string.Empty;

        [CrudField(Label = "Created Date", Order = 100, ShowInGrid = true, GridWidth = "150", HideInForm = true)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [CrudField(Label = "Created By", Order = 101, ShowInGrid = false, HideInForm = true)]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
