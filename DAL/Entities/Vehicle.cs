namespace ERP.Models.Entities;

/// <summary>
/// Vehicle entity for managing fleet vehicles
/// </summary>
public class Vehicle
{
    public int Id { get; set; }
    public int VehicleID { get; set; }
    public string? VehicleNumber { get; set; }
    public string? VehicleType { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public int? YearOfManufacture { get; set; }
    public int? ModelYear { get; set; }
    public decimal? Capacity { get; set; }
    public string? ChassisNumber { get; set; }
    public string? EngineNumber { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }
    public DateTime? InsuranceExpiry { get; set; }
    public DateTime? FitnessExpiryDate { get; set; }
    public DateTime? PermitExpiryDate { get; set; }
    public string? OwnerName { get; set; }
    public string? CreatedBy { get; set; }
    public bool? IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
