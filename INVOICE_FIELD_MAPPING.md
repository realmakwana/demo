# Invoice Entity Field Mapping - Quick Reference

## 📊 Field Name Changes

### Invoice Entity
```
OLD                  →  NEW
─────────────────────────────────────
Id                   →  InvoiceID
InvoiceNumber        →  invoice_no
InvoiceDate          →  invoice_date
CustomerName         →  CustomerID (int FK)
CustomerAddress      →  [REMOVED]
CustomerPhone        →  [REMOVED]
CustomerEmail        →  [REMOVED]
SubTotal             →  [REMOVED]
TaxAmount            →  [REMOVED]
DiscountAmount       →  [REMOVED]
TotalAmount          →  [REMOVED]
Notes                →  [REMOVED]
```

### InvoiceItem Entity
```
OLD                  →  NEW
─────────────────────────────────────
Id                   →  LineItemID
InvoiceId            →  InvoiceId (unchanged)
ItemName             →  item_name
Description          →  remarks
Category             →  [REMOVED]
Quantity             →  qty
UnitPrice            →  rate
[NEW]                →  discount
Amount               →  amount
```

---

## 🔄 Code Migration Examples

### Creating an Invoice

#### Before:
```csharp
var invoice = new Invoice
{
    InvoiceNumber = "INV-001",
    InvoiceDate = DateTime.Now,
    CustomerName = "ABC Logistics",
    CustomerPhone = "9876543210",
    CustomerEmail = "abc@example.com",
    CustomerAddress = "123 Main St",
    SubTotal = 50000,
    TaxAmount = 9000,
    TotalAmount = 59000
};
```

#### After:
```csharp
var invoice = new Invoice
{
    invoice_no = "INV-001",
    invoice_date = DateTime.Now,
    CustomerID = 5  // FK reference to tblCustomer
};
```

---

### Adding Line Items

#### Before:
```csharp
var item = new InvoiceItem
{
    ItemName = "Freight Service",
    Description = "Mumbai to Delhi",
    Category = "Freight",
    Quantity = 1,
    UnitPrice = 50000,
    Amount = 50000
};
```

#### After:
```csharp
var item = new InvoiceItem
{
    item_name = "Freight Service",
    remarks = "Mumbai to Delhi",
    qty = 1,
    rate = 50000,
    discount = 0,
    amount = 50000  // (qty * rate) - discount
};
```

---

### Querying Invoices

#### Before:
```csharp
// Get by ID
var invoice = await context.Invoices
    .FirstOrDefaultAsync(i => i.Id == id);

// Get by number
var invoice = await context.Invoices
    .FirstOrDefaultAsync(i => i.InvoiceNumber == number);

// Get by customer
var invoices = await context.Invoices
    .Where(i => i.CustomerName.Contains(name))
    .ToListAsync();

// Order by date
var invoices = await context.Invoices
    .OrderByDescending(i => i.InvoiceDate)
    .ToListAsync();
```

#### After:
```csharp
// Get by ID
var invoice = await context.Invoices
    .FirstOrDefaultAsync(i => i.InvoiceID == id);

// Get by number
var invoice = await context.Invoices
    .FirstOrDefaultAsync(i => i.invoice_no == number);

// Get by customer
var invoices = await context.Invoices
    .Where(i => i.CustomerID == customerId)
    .ToListAsync();

// Order by date
var invoices = await context.Invoices
    .OrderByDescending(i => i.invoice_date)
    .ToListAsync();
```

---

### Razor Binding

#### Before:
```razor
<SfTextBox @bind-Value="currentInvoice.InvoiceNumber" />
<SfDatePicker @bind-Value="currentInvoice.InvoiceDate" />
<AutoSuggestBox @bind-Value="currentInvoice.CustomerName" />

<!-- Line Items -->
<SfTextBox @bind-Value="newItem.ItemName" />
<SfTextBox @bind-Value="newItem.Description" />
<SfDropDownList @bind-Value="newItem.Category" />
<SfNumericTextBox @bind-Value="newItem.Quantity" />
<SfNumericTextBox @bind-Value="newItem.UnitPrice" />

<!-- Totals -->
<div>SubTotal: @invoice.SubTotal</div>
<div>Tax: @invoice.TaxAmount</div>
<div>Total: @invoice.TotalAmount</div>
```

#### After:
```razor
<SfTextBox @bind-Value="currentInvoice.invoice_no" />
<SfDatePicker @bind-Value="currentInvoice.invoice_date" />
<SfDropDownList TValue="int" @bind-Value="currentInvoice.CustomerID" 
                DataSource="@customers">
    <DropDownListFieldSettings Value="CustomerID" Text="CustomerName" />
</SfDropDownList>

<!-- Line Items -->
<SfTextBox @bind-Value="newItem.item_name" />
<SfTextBox @bind-Value="newItem.remarks" />
<SfNumericTextBox @bind-Value="newItem.qty" />
<SfNumericTextBox @bind-Value="newItem.rate" />
<SfNumericTextBox @bind-Value="newItem.discount" />

<!-- Total -->
<div>Total: @CalculateTotal()</div>
```

---

### Service Method Signatures

#### Before:
```csharp
Task<Invoice?> GetInvoiceByIdAsync(int id);
Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber);
Task<List<Invoice>> GetInvoicesByCustomerAsync(string customerName);
```

#### After:
```csharp
Task<Invoice?> GetInvoiceByIdAsync(int id);  // Same signature
Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber);  // Same signature
Task<List<Invoice>> GetInvoicesByCustomerAsync(int customerId);  // Changed parameter type
```

---

## 💡 Key Takeaways

1. **Normalized Design**: Customer data now stored in separate table, referenced by FK
2. **Simplified Calculations**: No built-in tax/total fields, calculated on-the-fly
3. **Snake Case**: Field names use snake_case (invoice_no, invoice_date, item_name, etc.)
4. **Discount Support**: New discount field added to line items
5. **Cleaner Structure**: Removed redundant fields, focused on core invoice data

---

## 🎯 Migration Checklist

When updating existing code:

- [ ] Replace `Id` with `InvoiceID`
- [ ] Replace `InvoiceNumber` with `invoice_no`
- [ ] Replace `InvoiceDate` with `invoice_date`
- [ ] Replace `CustomerName` string with `CustomerID` int
- [ ] Remove references to `CustomerAddress`, `CustomerPhone`, `CustomerEmail`
- [ ] Remove references to `SubTotal`, `TaxAmount`, `TotalAmount`
- [ ] Replace `ItemName` with `item_name`
- [ ] Replace `Description` with `remarks`
- [ ] Replace `Category` (removed)
- [ ] Replace `Quantity` with `qty`
- [ ] Replace `UnitPrice` with `rate`
- [ ] Add `discount` field handling
- [ ] Update amount calculation: `(qty * rate) - discount`

---

## 📝 Example: Complete Invoice Creation

```csharp
// Load customer
var customer = await CustomerService.GetCustomerByIdAsync(5);

// Create invoice
var invoice = new Invoice
{
    invoice_no = await InvoiceService.GenerateInvoiceNumberAsync(),
    invoice_date = DateTime.Now,
    CustomerID = customer.CustomerID
};

// Add line items
invoice.Items = new List<InvoiceItem>
{
    new InvoiceItem
    {
        item_name = "Freight Service",
        remarks = "Mumbai to Delhi",
        qty = 1,
        rate = 50000,
        discount = 2000,
        amount = 48000  // (1 * 50000) - 2000
    },
    new InvoiceItem
    {
        item_name = "Loading Charges",
        remarks = "Loading at Mumbai Port",
        qty = 1,
        rate = 5000,
        discount = 0,
        amount = 5000  // (1 * 5000) - 0
    }
};

// Save to database
await InvoiceService.CreateInvoiceAsync(invoice);

// Total: 48000 + 5000 = ₹53,000
```

---

## ✅ All Updated!

Your Invoice service and pages are now fully aligned with the new entity structure! 🚀
