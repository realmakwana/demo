using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities;

[Table("tblUserWiseRights")]
public class UserWiseRights : BaseEntity
{
    [Key]
    public int UserWiseRightsID { get; set; }

    public int UserID { get; set; }

    public int RightsID { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int? CreatedByUserID { get; set; }

    public string? Remarks { get; set; }

    // Note: SQL shows Created/Modified instead of CreatedDate/ModifiedDate
    // We will map them to BaseEntity properties or add new ones if they differ significantly
    [Column("Created")]
    public DateTime? CreatedAt { get; set; }

    [Column("Modified")]
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("UserID")]
    public virtual User? User { get; set; }
}
