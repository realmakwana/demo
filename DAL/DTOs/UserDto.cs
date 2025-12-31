using System.ComponentModel.DataAnnotations;

namespace TransportERP.Models.DTOs;

/// <summary>
/// User Data Transfer Object for API responses
/// </summary>
public class UserDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "User Name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "User Name must be between 3 and 50 characters")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Mobile { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = string.Empty;

    public string? Department { get; set; }
    public string? Designation { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? LastLoginDate { get; set; }
}
