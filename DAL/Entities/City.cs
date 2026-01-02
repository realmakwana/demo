namespace ERP.Models.Entities;

/// <summary>
/// City entity for location management
/// </summary>
public class City
{
    public int CityID { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Pincode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
