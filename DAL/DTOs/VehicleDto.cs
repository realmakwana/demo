using System;
using System.ComponentModel.DataAnnotations;
using TransportERP.Models.Attributes;

namespace TransportERP.Models.DTOs
{
    /// <summary>
    /// Example: Vehicle DTO with automatic CRUD configuration
    /// </summary>
    public class VehicleDto
    {
        [CrudField(Label = "ID", Order = 1, ShowInGrid = true, GridWidth = "80", HideInForm = true)]
        public int Id { get; set; }

        [CrudField(Label = "Vehicle Number", Order = 2, Required = true, FieldType = "Text", ShowInGrid = true, GridWidth = "150")]
        [Required(ErrorMessage = "Vehicle number is required")]
        [StringLength(20)]
        public string VehicleNumber { get; set; } = string.Empty;

        [CrudField(Label = "Vehicle Type", Order = 3, FieldType = "Dropdown", DataSource = "Truck,Trailer,Container,Tanker", ShowInGrid = true, GridWidth = "120")]
        public string VehicleType { get; set; } = "Truck";

        [CrudField(Label = "Capacity (Tons)", Order = 4, FieldType = "Number", Format = "N2", ShowInGrid = true, GridWidth = "120")]
        public decimal Capacity { get; set; }

        [CrudField(Label = "Model Year", Order = 5, FieldType = "Number", Format = "N0", ShowInGrid = true, GridWidth = "100")]
        public int ModelYear { get; set; }

        [CrudField(Label = "Purchase Date", Order = 6, FieldType = "Date", ShowInGrid = false)]
        public DateTime? PurchaseDate { get; set; }

        [CrudField(Label = "Insurance Expiry", Order = 7, FieldType = "Date", ShowInGrid = true, GridWidth = "150")]
        public DateTime? InsuranceExpiry { get; set; }

        [CrudField(Label = "Active", Order = 8, FieldType = "Checkbox", ShowInGrid = true, GridWidth = "80", GridTemplate = "ActiveBadge")]
        public bool IsActive { get; set; } = true;

        [CrudField(Label = "Remarks", Order = 9, FieldType = "TextArea", ShowInGrid = false)]
        public string Remarks { get; set; } = string.Empty;

        [CrudField(Label = "Created Date", Order = 100, ShowInGrid = true, GridWidth = "150", HideInForm = true)]
        public DateTime CreatedDate { get; set; }

        [CrudField(Label = "Created By", Order = 101, ShowInGrid = false, HideInForm = true)]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
