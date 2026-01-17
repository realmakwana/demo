# Quick Reference: Invoice Service Usage

## 🎯 How to Use Invoice Service in Your Code

### 1. Inject the Service
```csharp
[Inject] private IInvoiceService InvoiceService { get; set; } = default!;
```

### 2. Common Operations

#### Get All Invoices
```csharp
var invoices = await InvoiceService.GetAllInvoicesAsync();
// Returns List<Invoice> with all items included
```

#### Get Invoice by ID
```csharp
var invoice = await InvoiceService.GetInvoiceByIdAsync(invoiceId);
// Returns Invoice? (null if not found)
```

#### Get Invoice by Number
```csharp
var invoice = await InvoiceService.GetInvoiceByNumberAsync("INV-20260108-0001");
// Returns Invoice? (null if not found)
```

#### Create New Invoice
```csharp
var newInvoice = new Invoice
{
    InvoiceNumber = await InvoiceService.GenerateInvoiceNumberAsync(),
    InvoiceDate = DateTime.Now,
    CustomerName = "ABC Logistics",
    CustomerPhone = "9876543210",
    CustomerEmail = "contact@abc.com",
    Items = new List<InvoiceItem>
    {
        new InvoiceItem
        {
            ItemName = "Freight Service",
            Description = "Mumbai to Delhi",
            Category = "Freight",
            Quantity = 1,
            UnitPrice = 50000,
            Amount = 50000
        }
    }
};

// Calculate totals
newInvoice.SubTotal = newInvoice.Items.Sum(i => i.Amount);
newInvoice.TaxAmount = newInvoice.SubTotal * 0.18m;
newInvoice.TotalAmount = newInvoice.SubTotal + newInvoice.TaxAmount;

var created = await InvoiceService.CreateInvoiceAsync(newInvoice);
```

#### Update Invoice
```csharp
// Load existing invoice
var invoice = await InvoiceService.GetInvoiceByIdAsync(invoiceId);

if (invoice != null)
{
    // Modify invoice
    invoice.CustomerName = "Updated Name";
    
    // Update items
    invoice.Items.Add(new InvoiceItem
    {
        ItemName = "New Item",
        Category = "Loading",
        Quantity = 2,
        UnitPrice = 5000,
        Amount = 10000
    });
    
    // Recalculate totals
    invoice.SubTotal = invoice.Items.Sum(i => i.Amount);
    invoice.TaxAmount = invoice.SubTotal * 0.18m;
    invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;
    
    var updated = await InvoiceService.UpdateInvoiceAsync(invoice);
}
```

#### Delete Invoice
```csharp
var success = await InvoiceService.DeleteInvoiceAsync(invoiceId);
if (success)
{
    // Invoice deleted successfully
}
else
{
    // Invoice not found
}
```

#### Search by Customer
```csharp
var customerInvoices = await InvoiceService.GetInvoicesByCustomerAsync("ABC Logistics");
// Returns all invoices for customer (partial match)
```

#### Search by Date Range
```csharp
var startDate = new DateTime(2026, 1, 1);
var endDate = new DateTime(2026, 1, 31);
var invoices = await InvoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);
// Returns invoices between dates
```

#### Get Total Count
```csharp
var totalCount = await InvoiceService.GetTotalInvoicesCountAsync();
// Returns total number of invoices
```

#### Generate Invoice Number
```csharp
var nextInvoiceNumber = await InvoiceService.GenerateInvoiceNumberAsync();
// Returns: "INV-20260108-0001" (format: INV-YYYYMMDD-XXXX)
```

---

## 💡 Best Practices

### 1. Always Use Try-Catch
```csharp
try
{
    var invoices = await InvoiceService.GetAllInvoicesAsync();
}
catch (Exception ex)
{
    ToastService.ShowToast($"Error: {ex.Message}", ToastLevel.Error);
}
```

### 2. Calculate Totals Before Saving
```csharp
private void UpdateTotals(Invoice invoice)
{
    invoice.SubTotal = invoice.Items.Sum(i => i.Amount);
    invoice.TaxAmount = invoice.SubTotal * 0.18m; // 18% tax
    invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;
}
```

### 3. Validate Before Creating
```csharp
if (!invoice.Items.Any())
{
    ToastService.ShowToast("Add at least one item", ToastLevel.Warning);
    return;
}
```

### 4. Set InvoiceId for Items
```csharp
foreach (var item in invoice.Items)
{
    item.InvoiceId = invoice.Id; // Important for updates
}
```

---

## 🔍 Example: Complete Invoice Creation Flow

```csharp
private async Task CreateCompleteInvoice()
{
    try
    {
        // 1. Generate invoice number
        var invoiceNumber = await InvoiceService.GenerateInvoiceNumberAsync();
        
        // 2. Create invoice
        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.Now,
            CustomerName = "ABC Logistics",
            CustomerPhone = "9876543210",
            CustomerEmail = "contact@abc.com",
            CustomerAddress = "123 Main St, Mumbai"
        };
        
        // 3. Add items
        var items = new List<InvoiceItem>
        {
            new InvoiceItem
            {
                ItemName = "Freight Service",
                Description = "Mumbai to Delhi",
                Category = "Freight",
                Quantity = 1,
                UnitPrice = 50000,
                Amount = 50000
            },
            new InvoiceItem
            {
                ItemName = "Loading Charges",
                Description = "Loading at Mumbai",
                Category = "Loading",
                Quantity = 1,
                UnitPrice = 5000,
                Amount = 5000
            }
        };
        
        invoice.Items = items;
        
        // 4. Calculate totals
        invoice.SubTotal = items.Sum(i => i.Amount);
        invoice.TaxAmount = invoice.SubTotal * 0.18m;
        invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;
        
        // 5. Save to database
        var created = await InvoiceService.CreateInvoiceAsync(invoice);
        
        // 6. Show success message
        ToastService.ShowToast(
            $"Invoice {created.InvoiceNumber} created successfully!", 
            ToastLevel.Success
        );
        
        // 7. Reload list
        await LoadInvoices();
    }
    catch (Exception ex)
    {
        ToastService.ShowToast($"Error creating invoice: {ex.Message}", ToastLevel.Error);
    }
}
```

---

## 📊 Service Methods Summary

| Method | Returns | Description |
|--------|---------|-------------|
| `GetAllInvoicesAsync()` | `Task<List<Invoice>>` | All invoices with items |
| `GetInvoiceByIdAsync(id)` | `Task<Invoice?>` | Single invoice by ID |
| `GetInvoiceByNumberAsync(number)` | `Task<Invoice?>` | Single invoice by number |
| `CreateInvoiceAsync(invoice)` | `Task<Invoice>` | Create new invoice |
| `UpdateInvoiceAsync(invoice)` | `Task<Invoice>` | Update existing invoice |
| `DeleteInvoiceAsync(id)` | `Task<bool>` | Delete invoice |
| `GetTotalInvoicesCountAsync()` | `Task<int>` | Total count |
| `GetInvoicesByCustomerAsync(name)` | `Task<List<Invoice>>` | Filter by customer |
| `GetInvoicesByDateRangeAsync(start, end)` | `Task<List<Invoice>>` | Filter by date |
| `GenerateInvoiceNumberAsync()` | `Task<string>` | Next invoice number |

---

## 🎯 Ready to Use!

Your Invoice service is now fully integrated and ready for production use! 🚀
