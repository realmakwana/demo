using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportERP.Models.Entities;

[Table("tblMenu")]
public class Menu : BaseEntity
{
    [Key]
    public int MenuID { get; set; }

    [Required]
    [MaxLength(100)]
    public string MenuName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string MenuDispName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? MenuUrl { get; set; }

    [MaxLength(100)]
    public string? ClassName { get; set; }

    public string? Remarks { get; set; }

    public int? CompanyID { get; set; }

    public int ParentMenuID { get; set; } = 0;

    [MaxLength(100)]
    public string? FormName { get; set; }

    public decimal? Sequence { get; set; }

    public bool? Restricted { get; set; }

    public bool? VisibleToMenu { get; set; }

    [ForeignKey("ParentMenuID")]
    public virtual Menu? ParentMenu { get; set; }
    
    public virtual ICollection<Menu> SubMenus { get; set; } = new List<Menu>();
}
