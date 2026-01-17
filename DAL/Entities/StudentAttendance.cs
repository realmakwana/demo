using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities
{
    [Table("StudentAttendance")]
    public class StudentAttendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AttendanceID")]
        public int AttendanceID { get; set; }

        [Required]
        [Column("StudentID")]
        public int StudentID { get; set; }

        [Required]
        [Column("AttendanceDate", TypeName = "date")]
        public DateTime AttendanceDate { get; set; }

        [Required]
        [Column("IsPresent")]
        public bool IsPresent { get; set; }

        [ForeignKey("StudentID")]
        public Student? Student { get; set; }

        [NotMapped]
        public string? StudentName { get; set; }
    }
}
