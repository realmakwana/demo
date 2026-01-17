using Microsoft.AspNetCore.Components;
using ERP.Models.Entities;
using ERP.Components.Shared.UI;
using ERP.Models.Services;

namespace ERP.Pages.Transaction
{
    /// <summary>
    /// Code-behind for Invoices page component
    /// </summary>
    public partial class Invoices : ComponentBase
    {
        [Inject] private IInvoiceService InvoiceService { get; set; } = default!;
        [Inject] private ICustomerService CustomerService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        /// <summary>
        /// Breadcrumb navigation items
        /// </summary>
        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Transaction", Url = "#" },
            new() { Label = "Invoices", Url = "/invoices" }
        };

        private List<Invoice> invoices = new();
        private Invoice? currentInvoice;
        private List<InvoiceItem> currentInvoiceItems = new();
        private InvoiceItem newItem = new() { qty = 1, rate = 0, discount = 0 };
        private bool showForm = false;
        private UserWiseMenu? currentRights;
        private List<Customer> customers = new();

        /// <summary>
        /// Initializes the component and loads data
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadInvoices();
            await LoadRights();
            await LoadCustomers();
        }

        /// <summary>
        /// Loads user rights for the current page
        /// </summary>
        private async Task LoadRights()
        {
            if (AuthService.IsAuthenticated)
            {
                var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
                var path = "/" + relativePath.Split('?')[0];
                currentRights = await MenuService.GetUserMenuRightsAsync(AuthService.UserId, path);
            }
        }

        /// <summary>
        /// Loads all invoices from the database
        /// </summary>
        private async Task LoadInvoices()
        {
            try
            {
                invoices = await InvoiceService.GetAllInvoicesAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading invoices: {ex.Message}", ToastLevel.Error);
                invoices = new List<Invoice>();
            }
        }

        /// <summary>
        /// Loads customers for dropdown
        /// </summary>
        private async Task LoadCustomers()
        {
            try
            {
                customers = await CustomerService.GetActiveCustomersAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading customers: {ex.Message}", ToastLevel.Warning);
                customers = new List<Customer>();
            }
        }

        /// <summary>
        /// Opens the form to add a new invoice
        /// </summary>
        private async void OpenAddForm()
        {
            try
            {
                var invoiceNumber = await InvoiceService.GenerateInvoiceNumberAsync();
                currentInvoice = new Invoice
                {
                    invoice_date = DateTime.Now,
                    invoice_no = invoiceNumber,
                    CustomerID = 0
                };
                currentInvoiceItems = new List<InvoiceItem>();
                showForm = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error opening form: {ex.Message}", ToastLevel.Error);
            }
        }

        /// <summary>
        /// Opens the form to edit an existing invoice
        /// </summary>
        /// <param name="invoice">Invoice entity to edit</param>
        private void OpenEditForm(Invoice invoice)
        {
            currentInvoice = new Invoice
            {
                InvoiceID = invoice.InvoiceID,
                invoice_no = invoice.invoice_no,
                invoice_date = invoice.invoice_date,
                CustomerID = invoice.CustomerID
            };
            
            currentInvoiceItems = invoice.Items.Select(i => new InvoiceItem
            {
                LineItemID = i.LineItemID,
                InvoiceId = i.InvoiceId,
                item_name = i.item_name,
                remarks = i.remarks,
                qty = i.qty,
                rate = i.rate,
                discount = i.discount,
                amount = i.amount
            }).ToList();
            
            showForm = true;
        }

        /// <summary>
        /// Closes the form and resets state
        /// </summary>
        private void CloseForm()
        {
            showForm = false;
            currentInvoice = null;
            currentInvoiceItems = new List<InvoiceItem>();
            newItem = new InvoiceItem { qty = 1, rate = 0, discount = 0 };
        }

        /// <summary>
        /// Adds a line item to the current invoice
        /// </summary>
        private void AddLineItem()
        {
            if (string.IsNullOrWhiteSpace(newItem.item_name))
            {
                ToastService.ShowToast("Item name is required", ToastLevel.Warning);
                return;
            }

            newItem.LineItemID = currentInvoiceItems.Any() ? currentInvoiceItems.Max(i => i.LineItemID) + 1 : 1;
            newItem.amount = (newItem.qty * newItem.rate) - newItem.discount;
            currentInvoiceItems.Add(newItem);
            
            newItem = new InvoiceItem { qty = 1, rate = 0, discount = 0 };
        }

        /// <summary>
        /// Removes a line item from the current invoice
        /// </summary>
        /// <param name="item">Item to remove</param>
        private void RemoveLineItem(InvoiceItem item)
        {
            currentInvoiceItems.Remove(item);
        }

        /// <summary>
        /// Calculates total amount from line items
        /// </summary>
        /// <returns>Total amount</returns>
        private decimal CalculateTotal()
        {
            return currentInvoiceItems.Sum(i => i.amount);
        }

        /// <summary>
        /// Saves the invoice (create or update)
        /// </summary>
        private async Task SaveInvoice()
        {
            if (currentInvoice == null) return;

            if (!currentInvoiceItems.Any())
            {
                ToastService.ShowToast("Please add at least one item to the invoice", ToastLevel.Warning);
                return;
            }

            if (currentInvoice.CustomerID == 0)
            {
                ToastService.ShowToast("Please select a customer", ToastLevel.Warning);
                return;
            }

            try
            {
                currentInvoice.Items = currentInvoiceItems;

                if (currentInvoice.InvoiceID == 0)
                {
                    currentInvoice.CreatedBy = AuthService.UserId;
                    currentInvoice.IsActive = true;
                    await InvoiceService.CreateInvoiceAsync(currentInvoice);
                    ToastService.ShowToast("Invoice created successfully!", ToastLevel.Success);
                }
                else
                {
                    currentInvoice.ModifiedBy = AuthService.UserId;
                    await InvoiceService.UpdateInvoiceAsync(currentInvoice);
                    ToastService.ShowToast("Invoice updated successfully!", ToastLevel.Success);
                }

                await LoadInvoices();
                CloseForm();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving invoice: {ex.Message}", ToastLevel.Error);
            }
        }
        /// <summary>
        /// Moves an existing line item back to the entry form for modification.
        /// </summary>
        private void EditLineItem(InvoiceItem item)
        {
            if (item != null)
            {
                newItem = new InvoiceItem
                {
                    LineItemID = item.LineItemID,
                    item_name = item.item_name,
                    remarks = item.remarks,
                    qty = item.qty,
                    rate = item.rate,
                    discount = item.discount,
                    amount = item.amount,
                    InvoiceId = item.InvoiceId
                };
                currentInvoiceItems.Remove(item);

                StateHasChanged();
                ToastService.ShowToast("Item loaded for editing. Modify and click 'Add' again.", ToastLevel.Info);
            }
        }

        /// <summary>
        /// Deletes an invoice
        /// </summary>
        /// <param name="invoice">Invoice entity to delete</param>
        private async Task DeleteInvoice(Invoice invoice)
        {
            try
            {
                var success = await InvoiceService.DeleteInvoiceAsync(invoice.InvoiceID);
                if (success)
                {
                    ToastService.ShowToast("Invoice deleted successfully!", ToastLevel.Success);
                    await LoadInvoices();
                }
                else
                {
                    ToastService.ShowToast("Invoice not found!", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error deleting invoice: {ex.Message}", ToastLevel.Error);
            }
        }
    }
}
