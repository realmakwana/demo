using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities;

[Table("tblUserWiseMenu")]
public class UserWiseMenu : BaseEntity
{
    [Key]
    public int UserWiseMenuID { get; set; }

    public int UserID { get; set; }

    public int MenuID { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public bool IsShow { get; set; }

    public bool IsVerify { get; set; }

    public bool IsAdd { get; set; }

    public bool IsEdit { get; set; }

    public bool IsDelete { get; set; }

    public bool IsPrint { get; set; }

    public bool IsExport { get; set; }

    public TimeSpan? DayStartTime { get; set; }

    public TimeSpan? DayEndTime { get; set; }

    public string? Remarks { get; set; }

    public int? PastRecordAllowedDays { get; set; }

    [ForeignKey("MenuID")]
    public virtual Menu? Menu { get; set; }

    [ForeignKey("UserID")]
    public virtual User? User { get; set; }
}
