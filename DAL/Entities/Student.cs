using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities;

/// <summary>
/// Student Entity - Maps to tblStudent table in database
/// </summary>
[Table("tblStudent")]
public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("StudentID")]
    public int StudentID { get; set; }

    [Required(ErrorMessage ="Student name is required!")]
    [MaxLength(100)]
    [Column("StudentName")]
    public string StudentName { get; set; } = string.Empty;

    [MaxLength(15)]
    [Column("PhoneNumber")]
    public string? PhoneNumber { get; set; }

    [Column("IsActive")]
    public bool? IsActive { get; set; } = true;

    [NotMapped]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [NotMapped]
    public int? CreatedBy { get; set; }

    [NotMapped]
    public DateTime? ModifiedDate { get; set; }

    [NotMapped]
    public int? ModifiedBy { get; set; }
}
