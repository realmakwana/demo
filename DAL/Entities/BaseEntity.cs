namespace ERP.Models.Entities;

/// <summary>
/// Base entity class with common properties for all database entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Last modification date
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Created by user ID
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// Modified by user ID
    /// </summary>
    public int? ModifiedBy { get; set; }
}
