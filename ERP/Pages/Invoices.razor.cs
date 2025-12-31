using Microsoft.AspNetCore.Components;
using TransportERP.Models.DTOs;
using ERP.Components.Shared.CRUD;

namespace ERP.Pages
{
    public partial class Invoices : ComponentBase
    {
        private TransactionPage<InvoiceDto, InvoiceItemDto>? crudPage;
        private List<InvoiceDto> invoices = new();
        private InvoiceDto currentInvoice => crudPage?.GetCurrentHeader() ?? new InvoiceDto();
        private InvoiceItemDto newItem = new() { Quantity = 1, UnitPrice = 0 };
        private List<string> categories = new() { "Freight", "Loading", "Unloading", "Detention", "Other" };

        protected override void OnInitialized()
        {
            GenerateDummyCustomers();
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            var random = new Random();
            var customerNames = new[] { "ABC Logistics", "XYZ Traders", "PMP Transport", "Sharma Carriers", "Gupta Freight", 
                                       "Fast Movers Ltd", "Quick Transport", "Reliable Logistics", "Express Carriers", "Swift Freight" };
            var cities = new[] { "Mumbai", "Delhi", "Bangalore", "Chennai", "Kolkata", "Pune", "Hyderabad", "Ahmedabad" };
            var categories = new[] { "Freight", "Loading", "Unloading", "Detention", "Other" };
            
            invoices = new List<InvoiceDto>();
            
            for (int i = 1; i <= 20; i++)
            {
                var customerName = customerNames[random.Next(customerNames.Length)];
                var city = cities[random.Next(cities.Length)];
                var itemCount = random.Next(1, 4);
                var items = new List<InvoiceItemDto>();
                
                for (int j = 1; j <= itemCount; j++)
                {
                    var unitPrice = random.Next(10000, 100000);
                    var quantity = random.Next(1, 5);
                    items.Add(new InvoiceItemDto
                    {
                        Id = j,
                        ItemName = $"Service {j}",
                        Description = $"{city} to {cities[random.Next(cities.Length)]}",
                        Category = categories[random.Next(categories.Length)],
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        Amount = unitPrice * quantity
                    });
                }
                
                var subtotal = items.Sum(x => x.Amount);
                var tax = subtotal * 0.18m;
                
                invoices.Add(new InvoiceDto
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

        private void HandleLineItemAdded(InvoiceItemDto item)
        {
            if (string.IsNullOrWhiteSpace(newItem.ItemName)) return;

            if (newItem.Quantity <= 0) newItem.Quantity = 1;

            newItem.Amount = newItem.Quantity * newItem.UnitPrice;
            crudPage?.AddItemToList(newItem);
            newItem = new InvoiceItemDto() { Quantity = 1, UnitPrice = 0 };
            StateHasChanged();
        }

        private void HandleLineItemRemoved(InvoiceItemDto item) => StateHasChanged();

        private void SaveInvoice((InvoiceDto header, List<InvoiceItemDto> items) data)
        {
            data.header.Items = data.items;
            data.header.SubTotal = CalculateSubtotal();
            data.header.TaxAmount = CalculateTax();
            data.header.TotalAmount = CalculateTotal();

            if (data.header.Id == 0)
            {
                data.header.Id = invoices.Any() ? invoices.Max(i => i.Id) + 1 : 1;
                invoices.Add(data.header);
            }
            else
            {
                var index = invoices.FindIndex(i => i.Id == data.header.Id);
                if (index >= 0) invoices[index] = data.header;
            }
        }

        private void DeleteInvoice(InvoiceDto invoice) => invoices.Remove(invoice);

        private decimal CalculateSubtotal() => crudPage?.GetCurrentLineItems()?.Sum(i => i.Amount) ?? 0;
        private decimal CalculateTax() => CalculateSubtotal() * 0.18m;
        private decimal CalculateTotal() => CalculateSubtotal() + CalculateTax();

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
            await Task.Delay(300);
            if (string.IsNullOrWhiteSpace(searchText)) return Enumerable.Empty<string>();
            return dummyCustomers.Where(c => c.Contains(searchText, StringComparison.OrdinalIgnoreCase)).Take(50);
        }
    }
}
