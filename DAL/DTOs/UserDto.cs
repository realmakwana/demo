using System.ComponentModel.DataAnnotations;
using ERP.Models.Attributes;

namespace ERP.Models.DTOs;

/// <summary>
/// User Data Transfer Object - Consolidated from UserViewModel
/// </summary>
public class UserDto
{
    [CrudField(Label = "User ID", Order = 1, ShowInGrid = false, GridWidth = "80", HideInForm = true)]
    public int UserID { get; set; }

    [CrudField(Label = "User Category ID", Order = 2, Required = true, ShowInGrid = false, FieldType = "Number")]
    [Required(ErrorMessage = "User Category is required")]
    public int UserCategoryID { get; set; }

    [CrudField(Label = "User Name", Order = 3, Required = true, ShowInGrid = true, GridWidth = "150")]
    [Required(ErrorMessage = "User Name is required")]
    [StringLength(150, ErrorMessage = "User Name cannot exceed 150 characters")]
    public string UserName { get; set; } = string.Empty;

    [CrudField(Label = "Password", Order = 4, Required = true, ShowInGrid = false, FieldType = "Password")]
    [Required(ErrorMessage = "Password is required")]
    [StringLength(150, ErrorMessage = "Password cannot exceed 150 characters")]
    public string Password { get; set; } = string.Empty;

    [CrudField(Label = "Email ID", Order = 5, ShowInGrid = true, GridWidth = "200")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(500)]
    public string? EmailID { get; set; }

    [CrudField(Label = "Mobile No", Order = 6, ShowInGrid = true, GridWidth = "120")]
    [StringLength(10, ErrorMessage = "Mobile number must be 10 digits")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")]
    public string? MobileNo { get; set; }

    [CrudField(Label = "Company", Order = 7, ShowInGrid = true, GridWidth = "150", FieldType = "Dropdown", Required = false)]
    public int? CompanyID { get; set; }

    [CrudField(Label = "MAC Address", Order = 8, ShowInGrid = false)]
    [StringLength(1000)]
    public string? MAC { get; set; }

    [CrudField(Label = "Active", Order = 9, ShowInGrid = true, GridWidth = "100", FieldType = "Checkbox", GridTemplate = "StatusBadge")]
    public bool IsActive { get; set; } = true;

    // For backward compatibility - map to UserID
    public int Id 
    { 
        get => UserID; 
        set => UserID = value; 
    }

    // For backward compatibility - map to EmailID
    public string Email 
    { 
        get => EmailID ?? string.Empty; 
        set => EmailID = value; 
    }

    // For backward compatibility - map to MobileNo
    public string? Mobile 
    { 
        get => MobileNo; 
        set => MobileNo = value; 
    }

    // Additional properties for API compatibility
    public string? Role { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? LastLoginDate { get; set; }
}
