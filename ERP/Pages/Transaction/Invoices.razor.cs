using Microsoft.AspNetCore.Components;
using ERP.Models.Entities;
using ERP.Components.Shared.UI;
using ERP.Models.Services;

namespace ERP.Pages.Transaction
{
    public partial class Invoices : ComponentBase
    {
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Transaction", Url = "#" },
            new() { Label = "Invoices", Url = "/invoices" }
        };

        private List<Invoice> invoices = new();
        private Invoice? currentInvoice;
        private List<InvoiceItem> currentInvoiceItems = new();
        private InvoiceItem newItem = new() { Quantity = 1, UnitPrice = 0 };
        private bool showForm = false;
        private List<string> categories = new() { "Freight", "Loading", "Unloading", "Detention", "Other" };
        private UserWiseMenu? currentRights;

        protected override async Task OnInitializedAsync()
        {
            GenerateDummyCustomers();
            LoadInvoices();
            await LoadRights();
        }

        private async Task LoadRights()
        {
            if (AuthService.IsAuthenticated)
            {
                var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
                var path = "/" + relativePath.Split('?')[0];
                currentRights = await MenuService.GetUserMenuRightsAsync(AuthService.UserId, path);
            }
        }

        private void LoadInvoices()
        {
            var random = new Random();
            var customerNames = new[] { "ABC Logistics", "XYZ Traders", "PMP Transport", "Sharma Carriers", "Gupta Freight", 
                                       "Fast Movers Ltd", "Quick Transport", "Reliable Logistics", "Express Carriers", "Swift Freight" };
            var cities = new[] { "Mumbai", "Delhi", "Bangalore", "Chennai", "Kolkata", "Pune", "Hyderabad", "Ahmedabad" };
            var categoryList = new[] { "Freight", "Loading", "Unloading", "Detention", "Other" };
            
            invoices = new List<Invoice>();
            
            for (int i = 1; i <= 20; i++)
            {
                var customerName = customerNames[random.Next(customerNames.Length)];
                var city = cities[random.Next(cities.Length)];
                var itemCount = random.Next(1, 4);
                var items = new List<InvoiceItem>();
                
                for (int j = 1; j <= itemCount; j++)
                {
                    var unitPrice = random.Next(10000, 100000);
                    var quantity = random.Next(1, 5);
                    items.Add(new InvoiceItem
                    {
                        Id = j,
                        ItemName = $"Service {j}",
                        Description = $"{city} to {cities[random.Next(cities.Length)]}",
                        Category = categoryList[random.Next(categoryList.Length)],
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        Amount = unitPrice * quantity
                    });
                }
                
                var subtotal = items.Sum(x => x.Amount);
                var tax = subtotal * 0.18m;
                
                invoices.Add(new Invoice
                {
                    Id = i,
                    InvoiceNumber = $"INV-2024-{i:D3}",
                    InvoiceDate = DateTime.Now.AddDays(-random.Next(1, 60)),
                    CustomerName = customerName,
                    CustomerPhone = $"98765{random.Next(10000, 99999)}",
                    CustomerEmail = $"contact@{customerName.Replace(" ", "").ToLower()}.com",
                    CustomerAddress = $"{random.Next(1, 999)} Main Street, {city}",
                    Items = items,
                    SubTotal = subtotal,
                    TaxAmount = tax,
                    TotalAmount = subtotal + tax
                });
            }
        }

        private void OpenAddForm()
        {
            currentInvoice = new Invoice
            {
                InvoiceDate = DateTime.Now,
                InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{invoices.Count + 1}"
            };
            currentInvoiceItems = new List<InvoiceItem>();
            showForm = true;
        }

        private void OpenEditForm(Invoice invoice)
        {
            currentInvoice = new Invoice
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                CustomerName = invoice.CustomerName,
                CustomerPhone = invoice.CustomerPhone,
                CustomerEmail = invoice.CustomerEmail,
                CustomerAddress = invoice.CustomerAddress,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount
            };
            
            currentInvoiceItems = invoice.Items.Select(i => new InvoiceItem
            {
                Id = i.Id,
                ItemName = i.ItemName,
                Description = i.Description,
                Category = i.Category,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Amount = i.Amount
            }).ToList();
            
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentInvoice = null;
            currentInvoiceItems = new List<InvoiceItem>();
        }

        private void AddLineItem()
        {
            if (string.IsNullOrWhiteSpace(newItem.ItemName)) return;

            newItem.Id = currentInvoiceItems.Any() ? currentInvoiceItems.Max(i => i.Id) + 1 : 1;
            newItem.Amount = newItem.Quantity * newItem.UnitPrice;
            currentInvoiceItems.Add(newItem);
            
            newItem = new InvoiceItem { Quantity = 1, UnitPrice = 0 };
            UpdateTotals();
        }

        private void RemoveLineItem(InvoiceItem item)
        {
            currentInvoiceItems.Remove(item);
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            if (currentInvoice == null) return;
            currentInvoice.SubTotal = currentInvoiceItems.Sum(i => i.Amount);
            currentInvoice.TaxAmount = currentInvoice.SubTotal * 0.18m;
            currentInvoice.TotalAmount = currentInvoice.SubTotal + currentInvoice.TaxAmount;
        }

        private void SaveInvoice()
        {
            if (currentInvoice == null) return;

            currentInvoice.Items = currentInvoiceItems;
            UpdateTotals();

            if (currentInvoice.Id == 0)
            {
                currentInvoice.Id = invoices.Any() ? invoices.Max(i => i.Id) + 1 : 1;
                invoices.Add(currentInvoice);
                ToastService.ShowToast("Invoice created successfully!", ToastLevel.Success);
            }
            else
            {
                var index = invoices.FindIndex(i => i.Id == currentInvoice.Id);
                if (index >= 0)
                {
                    invoices[index] = currentInvoice;
                    ToastService.ShowToast("Invoice updated successfully!", ToastLevel.Success);
                }
            }
            
            CloseForm();
        }

        private void DeleteInvoice(Invoice invoice)
        {
            invoices.Remove(invoice);
            ToastService.ShowToast("Invoice deleted successfully!", ToastLevel.Success);
        }

        private List<string> dummyCustomers = new();

        private void GenerateDummyCustomers()
        {
            var prefixes = new[] { "Alpha", "Beta", "Gamma", "Delta", "Fast", "Super", "Mega", "Quick", "Smart", "Global", "Rapid", "Prime", "Elite", "Supreme" };
            var suffixes = new[] { "Logistics", "Transport", "Movers", "Carriers", "Freight", "Solutions", "Services", "Traders", "Enterprises", "Industries", "Supply Chain", "Express" };
            var locations = new[] { "Mumbai", "Delhi", "Bangalore", "Chennai", "Kolkata", "Pune", "Hyderabad", "Ahmedabad", "Jaipur", "Surat", "Indore", "Bhopal" };

            var random = new Random();
            for (int i = 0; i < 5000; i++)
            {
                var name = $"{prefixes[random.Next(prefixes.Length)]} {suffixes[random.Next(suffixes.Length)]} {locations[random.Next(locations.Length)]} ({random.Next(1000, 9999)})";
                dummyCustomers.Add(name);
            }
            
            dummyCustomers.AddRange(new[] { "ABC Logistics", "XYZ Traders", "PMP Transport", "Sharma Carriers", "Gupta Freight" });
            dummyCustomers.Sort();
        }

        private async Task<IEnumerable<string>> SearchCustomers(string searchText)
        {
            await Task.Delay(100);
            if (string.IsNullOrWhiteSpace(searchText)) return Enumerable.Empty<string>();
            return dummyCustomers.Where(c => c.Contains(searchText, StringComparison.OrdinalIgnoreCase)).Take(50);
        }
    }
}
