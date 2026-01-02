using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities;

/// <summary>
/// User Entity - Maps to tblUser table in database
/// </summary>
[Table("tblUser")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("UserID")]
    public int UserID { get; set; }

    [Required]
    [Column("UserCategoryID")]
    public int UserCategoryID { get; set; }

    [MaxLength(150)]
    [Column("UserName")]
    public string? UserName { get; set; }

    [MaxLength(150)]
    [Column("Password")]
    public string? Password { get; set; }

    [MaxLength(500)]
    [Column("EmailID")]
    public string? EmailID { get; set; }

    [MaxLength(10)]
    [Column("MobileNo")]
    public string? MobileNo { get; set; }

    [Column("IsActive")]
    public bool? IsActive { get; set; }

    [Column("CompanyID")]
    public int? CompanyID { get; set; }

    [MaxLength(1000)]
    [Column("MAC")]
    public string? MAC { get; set; }

    // Navigation property
    [ForeignKey("CompanyID")]
    public virtual Company? Company { get; set; }

    // For backward compatibility
    [NotMapped]
    public int UserId 
    { 
        get => UserID; 
        set => UserID = value; 
    }

    [NotMapped]
    public string Email 
    { 
        get => EmailID ?? string.Empty; 
        set => EmailID = value; 
    }

    [NotMapped]
    public string? Mobile 
    { 
        get => MobileNo; 
        set => MobileNo = value; 
    }

    // Legacy fields for compatibility
    [NotMapped]
    public string? Role { get; set; }

    [NotMapped]
    public string? Department { get; set; }

    [NotMapped]
    public string? Designation { get; set; }

    [NotMapped]
    public string? PasswordHash { get; set; }

    [NotMapped]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [NotMapped]
    public int? CreatedBy { get; set; }

    [NotMapped]
    public DateTime? ModifiedDate { get; set; }

    [NotMapped]
    public int? ModifiedBy { get; set; }
}
