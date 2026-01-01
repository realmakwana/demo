using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportERP.Models.Entities
{
    [Table("tblUserCategory")]
    public class UserCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserCategoryID")]
        public int UserCategoryID { get; set; }

        [MaxLength(150)]
        [Column("UserCategoryName")]
        public string? UserCategoryName { get; set; }

        [Column("IsActive")]
        public bool? IsActive { get; set; }
    }
}