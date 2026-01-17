using Microsoft.AspNetCore.Components;
using ERP.Models.Services;
using ERP.Components.Shared.UI;
using ERP.Components.Base;
using CustomerEntity = ERP.Models.Entities.Customer;
using ERP.Models.Entities;

namespace ERP.Pages.Master
{
    /// <summary>
    /// Code-behind for Customers page component
    /// </summary>
    public partial class Customers : CrudComponentBase
    {
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
            new() { Label = "Master", Url = "#" },
            new() { Label = "Customers", Url = "/customers" }
        };

        private List<CustomerEntity> customers = new();
        private CustomerEntity? currentCustomer;
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        /// <summary>
        /// Initializes the component and loads data
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadCustomers();
            await LoadRights();
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
        /// Loads all customers from the database
        /// </summary>
        private async Task LoadCustomers()
        {
            try
            {
                customers = await CustomerService.GetAllCustomersAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading customers: {ex.Message}", ToastLevel.Error);
                customers = new List<CustomerEntity>();
            }
        }

        /// <summary>
        /// Opens the form to add a new customer
        /// </summary>
        private void OpenAddForm()
        {
            currentCustomer = new CustomerEntity
            {
                IsActive = true
            };
            showForm = true;
        }

        /// <summary>
        /// Opens the form to edit an existing customer
        /// </summary>
        /// <param name="customer">Customer entity to edit</param>
        private void OpenEditForm(CustomerEntity customer)
        {
            currentCustomer = new CustomerEntity
            {
                CustomerID = customer.CustomerID,
                CustomerName = customer.CustomerName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                IsActive = customer.IsActive
            };
            showForm = true;
        }

        /// <summary>
        /// Closes the form and resets the current customer
        /// </summary>
        private void CloseForm()
        {
            showForm = false;
            currentCustomer = null;
        }

        /// <summary>
        /// Saves the customer (create or update)
        /// </summary>
        private async Task SaveCustomer()
        {
            if (currentCustomer == null) return;

            try
            {
                if (currentCustomer.CustomerID == 0)
                {
                    await CustomerService.CreateCustomerAsync(currentCustomer);
                    ToastService.ShowToast("Customer created successfully!", ToastLevel.Success);
                }
                else
                {
                    await CustomerService.UpdateCustomerAsync(currentCustomer);
                    ToastService.ShowToast("Customer updated successfully!", ToastLevel.Success);
                }

                await LoadCustomers();
                CloseForm();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving customer: {ex.Message}", ToastLevel.Error);
            }
        }

        /// <summary>
        /// Deletes a customer
        /// </summary>
        /// <param name="customer">Customer entity to delete</param>
        private async Task DeleteCustomer(CustomerEntity customer)
        {
            try
            {
                var success = await CustomerService.DeleteCustomerAsync(customer.CustomerID);
                if (success)
                {
                    ToastService.ShowToast("Customer deleted successfully!", ToastLevel.Success);
                    await LoadCustomers();
                }
                else
                {
                    ToastService.ShowToast("Customer not found!", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error deleting customer: {ex.Message}", ToastLevel.Error);
            }
        }
        protected override async Task OnSaveShortcut()

        {

            Console.WriteLine("Students.OnSaveShortcut called!");

            Console.WriteLine($"showForm: {showForm}, currentStudent: {(currentCustomer != null ? "exists" : "null")}");

            // Only save if form is visible and there's a current student

            if (showForm && currentCustomer != null)

            {

                Console.WriteLine("Attempting to save student...");

                // Validate the model before saving

                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(currentCustomer);

                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(

                    currentCustomer,

                    validationContext,

                    validationResults,

                    validateAllProperties: true);

                Console.WriteLine($"Validation result: {isValid}");

                if (isValid)

                {

                    await SaveCustomer();

                    // Force UI update to show grid

                    StateHasChanged();

                }

                else

                {

                    // Show validation errors

                    var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));

                    Console.WriteLine($"Validation errors: {errors}");

                    ToastService.ShowToast($"Validation failed: {errors}", ToastLevel.Warning);

                }

            }

            else

            {

                Console.WriteLine("Form not shown or student is null - not saving");

            }

        }

        protected override Task OnSearchShortcut()

        {

            return Task.CompletedTask;

        }

        protected override Task OnAddShortcut()

        {

            if (!showForm && (currentRights?.IsAdd ?? true))

            {

                OpenAddForm();

                StateHasChanged();

            }

            return Task.CompletedTask;

        }



    }
}





