using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities
{
    /// <summary>
    /// Represents a Customer entity in the ERP system
    /// </summary>
    [Table("tblCustomer")]
    public class Customer
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer
        /// </summary>
        [Key]
        [Column("CustomerID")]
        public int CustomerID { get; set; }

        /// <summary>
        /// Gets or sets the customer name
        /// </summary>
        [Required(ErrorMessage = "Customer Name is required")]
        [MaxLength(50, ErrorMessage = "Customer Name cannot exceed 50 characters")]
        [Column("CustomerName")]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer email address
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer phone number
        /// </summary>
        [Required(ErrorMessage = "Phone Number is required")]
        [MaxLength(50, ErrorMessage = "Phone Number cannot exceed 50 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the customer is active
        /// </summary>
        [Column("IsActive")]
        public bool? IsActive { get; set; } = true;
    }
}
