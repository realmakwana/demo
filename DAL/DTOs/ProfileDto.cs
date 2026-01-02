using System.ComponentModel.DataAnnotations;
using ERP.Models.Attributes;

namespace ERP.Models.DTOs;

/// <summary>
/// Profile Data Transfer Object - Consolidated from ProfileViewModel
/// </summary>
public class ProfileDto
{
    [CrudField(Label = "Full Name", Order = 1, Required = true, ShowInGrid = true, GridWidth = "200")]
    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [CrudField(Label = "Email", Order = 2, Required = true, ShowInGrid = true, GridWidth = "200")]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [CrudField(Label = "Mobile", Order = 3, Required = true, ShowInGrid = true, GridWidth = "120")]
    [Required(ErrorMessage = "Mobile Number is required")]
    [Phone]
    public string Mobile { get; set; } = string.Empty;

    [CrudField(Label = "Role", Order = 4, ShowInGrid = true, GridWidth = "150")]
    public string Role { get; set; } = string.Empty;

    [CrudField(Label = "Bio", Order = 5, ShowInGrid = false, FieldType = "Textarea")]
    public string? Bio { get; set; }
}
