using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Entities
{
    /// <summary>
    /// Represents an Invoice entity in the ERP system
    /// </summary>
    [Table("tblInvoice")]
    public class Invoice : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice
        /// </summary>
        [Key]
        [Column("InvoiceID")]
        public int InvoiceID { get; set; }

        /// <summary>
        /// Gets or sets the invoice number
        /// </summary>
        [Required(ErrorMessage = "Invoice number is required")]
        [StringLength(50, ErrorMessage = "Invoice number cannot exceed 50 characters")]
        [Column("invoice_no")]
        public string invoice_no { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the invoice date
        /// </summary>
        [Required(ErrorMessage = "Invoice date is required")]
        [Column("invoice_date")]
        public DateTime invoice_date { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the customer ID (foreign key)
        /// </summary>
        [Required(ErrorMessage = "Customer is required")]
        [Column("CustomerID")]
        public int CustomerID { get; set; }

        /// <summary>
        /// Gets or sets the list of invoice line items
        /// </summary>
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        // Inherited from BaseEntity:
        // - CreatedDate (DateTime)
        // - ModifiedDate (DateTime?)
        // - IsActive (bool)
        // - CreatedBy (int?)
        // - ModifiedBy (int?)
    }

    /// <summary>
    /// Represents an Invoice Line Item entity in the ERP system
    /// </summary>
    [Table("tblInvoicelineitems")]
    public class InvoiceItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the line item
        /// </summary>
        [Key]
        [Column("LineItemID")]
        public int LineItemID { get; set; }

        /// <summary>
        /// Gets or sets the invoice ID (foreign key)
        /// </summary>
        [Required(ErrorMessage = "Invoice ID is required")]
        [Column("InvoiceId")]
        public int InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets the item name
        /// </summary>
        [Required(ErrorMessage = "Item name is required")]
        [StringLength(200, ErrorMessage = "Item name cannot exceed 200 characters")]
        [Column("item_name")]
        public string item_name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the remarks/description
        /// </summary>
        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        [Column("remarks")]
        public string remarks { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Column("qty")]
        public int qty { get; set; } = 1;

        /// <summary>
        /// Gets or sets the rate/unit price
        /// </summary>
        [Required(ErrorMessage = "Rate is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
        [Column("rate")]
        public decimal rate { get; set; }

        /// <summary>
        /// Gets or sets the discount amount
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Discount cannot be negative")]
        [Column("discount")]
        public decimal discount { get; set; }

        /// <summary>
        /// Gets or sets the line item total amount
        /// </summary>
        [Column("amount")]
        public decimal amount { get; set; }

        // Inherited from BaseEntity:
        // - CreatedDate (DateTime)
        // - ModifiedDate (DateTime?)
        // - IsActive (bool)
        // - CreatedBy (int?)
        // - ModifiedBy (int?)
    }
}