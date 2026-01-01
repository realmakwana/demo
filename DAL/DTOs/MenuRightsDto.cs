using TransportERP.Models.Attributes;

namespace TransportERP.Models.DTOs;

/// <summary>
/// Menu Rights Data Transfer Object - Consolidated from MenuRightsViewModel
/// </summary>
public class MenuRightsDto
{
    [CrudField(Label = "User Wise Menu ID", Order = 1, ShowInGrid = false, GridWidth = "80", HideInForm = true)]
    public int UserWiseMenuID { get; set; }

    [CrudField(Label = "Menu ID", Order = 2, ShowInGrid = false, GridWidth = "80", HideInForm = true)]
    public int MenuID { get; set; }

    [CrudField(Label = "Menu Name", Order = 3, ShowInGrid = true, GridWidth = "200")]
    public string MenuDispName { get; set; } = string.Empty;

    [CrudField(Label = "Parent Menu ID", Order = 4, ShowInGrid = false, HideInForm = true)]
    public int ParentMenuID { get; set; }

    [CrudField(Label = "Show", Order = 5, ShowInGrid = true, GridWidth = "80", FieldType = "Checkbox")]
    public bool IsShow { get; set; }

    [CrudField(Label = "Add", Order = 6, ShowInGrid = true, GridWidth = "80", FieldType = "Checkbox")]
    public bool IsAdd { get; set; }

    [CrudField(Label = "Edit", Order = 7, ShowInGrid = true, GridWidth = "80", FieldType = "Checkbox")]
    public bool IsEdit { get; set; }

    [CrudField(Label = "Delete", Order = 8, ShowInGrid = true, GridWidth = "80", FieldType = "Checkbox")]
    public bool IsDelete { get; set; }

    [CrudField(Label = "Print", Order = 9, ShowInGrid = true, GridWidth = "80", FieldType = "Checkbox")]
    public bool IsPrint { get; set; }

    [CrudField(Label = "Export", Order = 10, ShowInGrid = true, GridWidth = "80", FieldType = "Checkbox")]
    public bool IsExport { get; set; }

    [CrudField(Label = "Verify", Order = 11, ShowInGrid = true, GridWidth = "80", FieldType = "Checkbox")]
    public bool IsVerify { get; set; }

    // For backward compatibility
    public int Id 
    { 
        get => UserWiseMenuID; 
        set => UserWiseMenuID = value; 
    }
}
