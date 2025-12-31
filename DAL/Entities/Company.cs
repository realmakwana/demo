using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportERP.Models.Entities;

/// <summary>
/// Company Entity - Maps to tblCompany table in database
/// </summary>
[Table("tblCompany")]
public class Company
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("CompanyID")]
    public int CompanyID { get; set; }

    [MaxLength(100)]
    [Column("CompanyName")]
    public string? CompanyName { get; set; }

    [MaxLength(300)]
    [Column("CompanyAddress")]
    public string? CompanyAddress { get; set; }

    [MaxLength(50)]
    [Column("GSTNO")]
    public string? GSTNO { get; set; }

    [MaxLength(50)]
    [Column("PANNO")]
    public string? PANNO { get; set; }

    [Column("IsActive")]
    public bool? IsActive { get; set; }

    [MaxLength(20)]
    [Column("PhoneNo")]
    public string? PhoneNo { get; set; }

    [MaxLength(20)]
    [Column("MobileNo")]
    public string? MobileNo { get; set; }

    [Column("StartDate")]
    public DateTime? StartDate { get; set; }

    [Column("EndDate")]
    public DateTime? EndDate { get; set; }

    [Column("CreatedDate")]
    public DateTime? CreatedDate { get; set; }

    [MaxLength(50)]
    [Column("ShortCode")]
    public string? ShortCode { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("Mail")]
    public string Mail { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("MailKey")]
    public string MailKey { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
