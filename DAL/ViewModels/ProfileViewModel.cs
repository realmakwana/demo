using System.ComponentModel.DataAnnotations;

namespace TransportERP.Models.ViewModels;

public class ProfileViewModel
{
    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile Number is required")]
    [Phone]
    public string Mobile { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
    public string? Bio { get; set; }
}
