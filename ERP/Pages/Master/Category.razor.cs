using ERP.Components.Base;
using ERP.Components.Shared.UI;
using ERP.Models.Entities;
using ERP.Models.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using CategoryEntity = ERP.Models.Entities.Category;
namespace ERP.Pages.Master
{
    /// <summary>
    /// Code-behind for Categorys page component
    /// </summary>
    public partial class Category : CrudComponentBase
    {
        [Inject] private ICategoryService CategoryService { get; set; } = default!;
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
            new() { Label = "Category", Url = "/Category" }
        };

        private List<CategoryEntity> Categories = new();
        private CategoryEntity? currentCategory;
        private List<CatType> CatTypeList = new();
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        private string categoryTypeSearchText = string.Empty;
        private bool isFormFirstRender = false;

        /// <summary>
        /// Initializes the component and loads data
        /// </summary>
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
        private async Task<IEnumerable<string>> SearchCatType(string searchText)
        {
            await Task.Delay(100);

            return CatTypeList
                .Where(x => string.IsNullOrWhiteSpace(searchText)
                         || x.CatTypeName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.CatTypeName);
        }
        private void OnCatTypeSelected(string selectedText)
        {
            categoryTypeSearchText = selectedText;
            var selected = CatTypeList
                .FirstOrDefault(x => string.Equals(x.CatTypeName, selectedText, StringComparison.OrdinalIgnoreCase));

            if (selected != null)
            {
                currentCategory.CatTypeID = selected.CatTypeID;
            }
            else
            {
                currentCategory.CatTypeID = 0;
            }
        }
       
        protected override async Task OnInitializedAsync()
        {
            CatTypeList = await Db.tblCatType.AsNoTracking().OrderBy(x => x.CatTypeName).ToListAsync();
            await LoadCategorys();
            await LoadRights();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            // Setup form navigation and auto-focus when form is shown
            if (showForm && isFormFirstRender)
            {
                isFormFirstRender = false;
                
                try
                {
                    // Use JavaScript to find and setup the form
                    await JSRuntime.InvokeVoidAsync("eval", @"
                        (function() {
                            const form = document.querySelector('.form-container form');
                            if (form) {
                                window.keyboardShortcuts.setupFormNavigation(form);
                                window.keyboardShortcuts.focusFirstFormElement(form);
                            }
                        })();
                    ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting up form navigation: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Loads all Categorys from the database
        /// </summary>
        private async Task LoadCategorys()
        {
            try
            {
                Categories = await CategoryService.GetAllCategorysAsync();

            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading Categorys: {ex.Message}", ToastLevel.Error);
                Categories = new List<CategoryEntity>();
            }
        }

        /// <summary>
        /// Opens the form to add a new Category
        /// </summary>
        private void OpenAddForm()
        {
            currentCategory = new CategoryEntity
            {
                IsActive = true
            };
            categoryTypeSearchText = string.Empty;

            showForm = true;
            isFormFirstRender = true;
        }

        /// <summary>
        /// Opens the form to edit an existing Category
        /// </summary>
        /// <param name="Category">Category entity to edit</param>
        private void OpenEditForm(CategoryEntity Category)
        {
            currentCategory = new CategoryEntity
            {
                CatID = Category.CatID,
                CatTypeID = Category.CatTypeID,
                CatName = Category.CatName,
                IsActive = Category.IsActive
            };
            
            var catType = CatTypeList.FirstOrDefault(x => x.CatTypeID == Category.CatTypeID);
            categoryTypeSearchText = catType?.CatTypeName ?? string.Empty;
            
            showForm = true;
            isFormFirstRender = true;
        }

        /// <summary>
        /// Closes the form and resets the current Category
        /// </summary>
        private void CloseForm()
        {
            showForm = false;
            currentCategory = null;
            categoryTypeSearchText = string.Empty;
        }

        /// <summary>
        /// Saves the Category (create or update)
        /// </summary>
        private async Task SaveCategory()
        {
            if (currentCategory == null) return;

            // Validate for blank or null values
            if (string.IsNullOrWhiteSpace(currentCategory.CatName))
            {
                ToastService.ShowToast("Category Name is required", ToastLevel.Warning);
                return;
            }

            if (currentCategory.CatTypeID <= 0)
            {
                ToastService.ShowToast("Please select a Category Type", ToastLevel.Warning);
                return;
            }

            try
            {
                if (currentCategory.CatID == 0)
                {
                    await CategoryService.CreateCategoryAsync(currentCategory);
                    ToastService.ShowToast("Category created successfully!", ToastLevel.Success);
                }
                else
                {
                    await CategoryService.UpdateCategoryAsync(currentCategory);
                    ToastService.ShowToast("Category updated successfully!", ToastLevel.Success);
                }

                await LoadCategorys();
                CloseForm();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving Category: {ex.Message}", ToastLevel.Error);
            }
        }

        /// <summary>
        /// Deletes a Category
        /// </summary>
        /// <param name="Category">Category entity to delete</param>
        private async Task DeleteCategory(CategoryEntity Category)
        {
            try
            {
                var success = await CategoryService.DeleteCategoryAsync(Category.CatID);
                if (success)
                {
                    ToastService.ShowToast("Category deleted successfully!", ToastLevel.Success);
                    await LoadCategorys();
                }
                else
                {
                    ToastService.ShowToast("Category not found!", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error deleting Category: {ex.Message}", ToastLevel.Error);
            }
        }
        protected override async Task OnSaveShortcut()

        {

            Console.WriteLine("Category.OnSaveShortcut called!");

            Console.WriteLine($"showForm: {showForm}, currentCategory: {(currentCategory != null ? "exists" : "null")}");

            // Only save if form is visible and there's a current Category

            if (showForm && currentCategory != null)

            {

                Console.WriteLine("Attempting to save Category...");

                // Validate the model before saving

                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(currentCategory);

                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(

                    currentCategory,

                    validationContext,

                    validationResults,

                    validateAllProperties: true);

                Console.WriteLine($"Validation result: {isValid}");

                if (isValid)

                {

                    await SaveCategory();

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

                Console.WriteLine("Form not shown or Category is null - not saving");

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





