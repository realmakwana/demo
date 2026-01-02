using System.ComponentModel.DataAnnotations;
using ERP.Models.Attributes;

namespace ERP.Models.DTOs;

/// <summary>
/// Company Data Transfer Object - Consolidated from CompanyViewModel
/// </summary>
public class CompanyDto
{
    [CrudField(Label = "Company ID", Order = 1, ShowInGrid = false, GridWidth = "80", HideInForm = true)]
    public int CompanyID { get; set; }

    [CrudField(Label = "Company Name", Order = 2, Required = true, ShowInGrid = true, GridWidth = "200")]
    [Required(ErrorMessage = "Company Name is required")]
    [StringLength(100, ErrorMessage = "Company Name cannot exceed 100 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [CrudField(Label = "Short Code", Order = 3, ShowInGrid = true, GridWidth = "100")]
    [StringLength(50, ErrorMessage = "Short Code cannot exceed 50 characters")]
    public string? ShortCode { get; set; }

    [CrudField(Label = "Company Address", Order = 4, ShowInGrid = false, FieldType = "Textarea")]
    [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters")]
    public string? CompanyAddress { get; set; }

    [CrudField(Label = "GST No", Order = 5, ShowInGrid = true, GridWidth = "150")]
    [StringLength(50, ErrorMessage = "GST No cannot exceed 50 characters")]
    [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid GST Number format")]
    public string? GSTNO { get; set; }

    [CrudField(Label = "PAN No", Order = 6, ShowInGrid = true, GridWidth = "120")]
    [StringLength(50, ErrorMessage = "PAN No cannot exceed 50 characters")]
    [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN Number format")]
    public string? PANNO { get; set; }

    [CrudField(Label = "Phone No", Order = 7, ShowInGrid = true, GridWidth = "120")]
    [StringLength(20, ErrorMessage = "Phone No cannot exceed 20 characters")]
    public string? PhoneNo { get; set; }

    [CrudField(Label = "Mobile No", Order = 8, ShowInGrid = true, GridWidth = "120")]
    [StringLength(20, ErrorMessage = "Mobile No cannot exceed 20 characters")]
    public string? MobileNo { get; set; }

    [CrudField(Label = "Email", Order = 9, Required = true, ShowInGrid = true, GridWidth = "200")]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string Mail { get; set; } = string.Empty;

    [CrudField(Label = "Email Password/Key", Order = 10, ShowInGrid = false, FieldType = "Password", Required = true)]
    [Required(ErrorMessage = "Email Key is required")]
    [StringLength(100)]
    public string MailKey { get; set; } = string.Empty;

    [CrudField(Label = "Start Date", Order = 11, ShowInGrid = true, GridWidth = "120", FieldType = "Date", Format = "dd MMM yyyy")]
    public DateTime? StartDate { get; set; }

    [CrudField(Label = "End Date", Order = 12, ShowInGrid = true, GridWidth = "120", FieldType = "Date", Format = "dd MMM yyyy")]
    public DateTime? EndDate { get; set; }

    [CrudField(Label = "Created Date", Order = 13, ShowInGrid = false, FieldType = "Date", Format = "dd MMM yyyy", ReadOnly = true, HideInForm =true)]
    public DateTime? CreatedDate { get; set; }

    [CrudField(Label = "Active", Order = 14, ShowInGrid = true, GridWidth = "100", FieldType = "Checkbox", GridTemplate = "StatusBadge")]
    public bool IsActive { get; set; } = true;

    // For backward compatibility
    public int Id 
    { 
        get => CompanyID; 
        set => CompanyID = value; 
    }
}
