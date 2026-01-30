using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities;

[Table("tblCategory")]
public class Category
{
    [Key]
    public Int32 CatID { get; set; }
    public String CatName { get; set; }
    public int CatTypeID { get; set; }
    public bool IsActive { get; set; }
}
