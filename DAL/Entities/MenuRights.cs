namespace ERP.Models.Entities;

/// <summary>
/// Menu rights entity for managing user permissions on menus
/// </summary>
public class MenuRights
{
    public int MenuRightsID { get; set; }
    public int MenuID { get; set; }
    public string? MenuDispName { get; set; }
    public int Level { get; set; } // Added for indentation
    public int ParentMenuID { get; set; }
    public int UserWiseMenuID { get; set; }
    public bool IsShow { get; set; }
    public bool IsAdd { get; set; }
    public bool IsEdit { get; set; }
    public bool IsDelete { get; set; }
    public bool IsPrint { get; set; }
    public bool IsExport { get; set; }
    public bool IsVerify { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsAllSelected 
    { 
        get => IsShow && IsAdd && IsEdit && IsDelete && IsPrint && IsExport && IsVerify;
        set 
        {
            IsShow = value;
            IsAdd = value;
            IsEdit = value;
            IsDelete = value;
            IsPrint = value;
            IsExport = value;
            IsVerify = value;
        }
    }
}
