using TransportERP.Models.Attributes;

namespace TransportERP.Models.DTOs;

/// <summary>
/// User Category Data Transfer Object - Consolidated from UserCategoryViewModel
/// </summary>
public class UserCategoryDto
{
    [CrudField(Label = "User Category ID", Order = 1, ShowInGrid = false, GridWidth = "80", HideInForm = true)]
    public int UserCategoryID { get; set; }

    [CrudField(Label = "User Category Name", Order = 2, ShowInGrid = true, GridWidth = "80", HideInForm = false)]
    public string? UserCategoryName { get; set; }

    [CrudField(Label = "Active", Order = 3, ShowInGrid = true, GridWidth = "100", FieldType = "Checkbox", GridTemplate = "StatusBadge")]
    public bool IsActive { get; set; } = true;

    // For backward compatibility
    public int Id 
    { 
        get => UserCategoryID; 
        set => UserCategoryID = value; 
    }
}
