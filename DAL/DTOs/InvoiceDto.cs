using System.ComponentModel.DataAnnotations;

namespace ERP.Models.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Invoice Number is required")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Invoice Date is required")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Customer Name is required")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer Address is required")]
        public string CustomerAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string CustomerEmail { get; set; } = string.Empty;

        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class InvoiceItemDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
