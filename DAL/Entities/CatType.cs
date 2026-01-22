using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities;

[Table("tblCatType")]
public class CatType
{
    public int CatTypeID { get; set; }
    public string CatTypeName { get; set; }
}

