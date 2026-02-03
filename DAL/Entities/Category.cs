using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities;

[Table("tblCategory")]
public class Category
{
    [Key]
    public Int32 CatID { get; set; }
    
    [Required(ErrorMessage = "Category Name is required")]
    public String CatName { get; set; }
    
    [Required(ErrorMessage = "Category Type is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a Category Type")]
    public int CatTypeID { get; set; }
    public bool IsActive { get; set; }
}
