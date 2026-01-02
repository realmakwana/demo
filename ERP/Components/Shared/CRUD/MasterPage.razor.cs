using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Calendars;
using System.Linq.Expressions;
using ERP.Components.Shared.UI;
using ERP.Models.Helpers;
using ERP.Models.Services;
using ERP.Models.Entities;

namespace ERP.Components.Shared.CRUD
{
    public partial class MasterPage<TEntity> : ComponentBase where TEntity : class, new()
    {
        [Parameter] public string Title { get; set; } = "Item";
        [Parameter] public string Subtitle { get; set; } = "";
        [Parameter] public string GridTitle { get; set; } = "Data List";
        [Parameter] public string? HelpText { get; set; }
        [Parameter] public List<TEntity> DataSource { get; set; } = new();
        [Parameter] public EventCallback<TEntity> OnSave { get; set; }
        [Parameter] public EventCallback<TEntity> OnDelete { get; set; }
        [Parameter] public Func<string, object, RenderFragment>? GridTemplateContent { get; set; }
        [Parameter] public RenderFragment<TEntity>? CustomActionsTemplate { get; set; }
        [Parameter] public Dictionary<string, List<string>>? DropdownDataSources { get; set; }

        // Modal Configuration
        [Parameter] public RenderFragment<TEntity>? ModalContent { get; set; }
        [Parameter] public string ModalWidth { get; set; } = "800px";
        [Parameter] public string ModalTitle { get; set; } = "Details";

        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private SfGrid<TEntity>? grid;
        private ConfirmationDialog confirmDialog = default!;
        private SfModal genericModal = default!;
        private EditForm? editForm;
        private TEntity currentItem = new();
        private TEntity? selectedModalItem;
        private bool showForm = false;
        private bool isEditMode = false;
        private bool isLoading = false;
        private List<CrudMetadataHelper.FieldMetadata> fieldMetadata = new();
        private List<AppBreadcrumbItem> breadcrumbItems = new();
        private ExportDialog exportDialog = default!;
        private UserWiseMenu? currentRights;
        private bool isAccessDenied = false;

        public void ShowLoading()
        {
            isLoading = true;
            StateHasChanged();
        }

        public void HideLoading()
        {
            isLoading = false;
            StateHasChanged();
        }





        protected override async Task OnInitializedAsync()
        {
            fieldMetadata = CrudMetadataHelper.GetFieldMetadata<TEntity>();
            UpdateBreadcrumb();
            await LoadRights();
        }

        private async Task LoadRights()
        {
            if (AuthService.IsAuthenticated)
            {
                var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
                var path = "/" + relativePath.Split('?')[0];
                currentRights = await MenuService.GetUserMenuRightsAsync(AuthService.UserId, path);
                
                if (currentRights == null || !currentRights.IsShow)
                {
                    // If it's the home page, we might want to allow it by default or check a specific home right
                    if (path != "/")
                    {
                        isAccessDenied = true;
                        StateHasChanged();
                    }
                }
            }
        }

        protected override void OnParametersSet()
        {
            // Show loading spinner if DataSource is empty (initial load)
            if (DataSource == null || DataSource.Count == 0)
            {
                isLoading = true;
            }
            else
            {
                // Hide loading spinner when data is available
                isLoading = false;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender && editForm?.EditContext != null)
            {
                editForm.EditContext.OnValidationStateChanged += (sender, args) => StateHasChanged();
            }
        }

        private void OnGridActionBegin(Syncfusion.Blazor.Grids.ActionEventArgs<TEntity> args)
        {
            // Show loading spinner when grid actions begin (paging, sorting, filtering, etc.)
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Paging ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Filtering ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Searching ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Refresh)
            {
                isLoading = true;
                StateHasChanged();
            }
        }

        private void OnGridActionComplete(Syncfusion.Blazor.Grids.ActionEventArgs<TEntity> args)
        {
            // Hide loading spinner when grid actions complete
            isLoading = false;
            StateHasChanged();
        }

        private void OnGridDataBound()
        {
            // Hide loading spinner when data is bound
            isLoading = false;
            StateHasChanged();
        }

        private Dictionary<string, Expression<Func<object>>> _expressionCache = new();

        private Expression<Func<object>> GetExpression(string propertyName)
        {
            // Create a proper expression tree: () => ((TEntity)constantInstance).PropertyName
            var propertyInfo = typeof(TEntity).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return () => new object();
            }
            
            // Create expression: () => currentItem.PropertyName (as object)
            var constantExpr = Expression.Constant(currentItem, typeof(TEntity));
            var propertyExpr = Expression.Property(constantExpr, propertyInfo);
            var convertExpr = Expression.Convert(propertyExpr, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(convertExpr);
            
            return lambda;
        }

        private void ClearExpressionCache() => _expressionCache.Clear();

        private void UpdateBreadcrumb()
        {
            breadcrumbItems = new List<AppBreadcrumbItem>
            {
                new AppBreadcrumbItem("Home", "/"),
                new AppBreadcrumbItem(Title, null)
            };

            if (showForm)
            {
                breadcrumbItems.Add(new AppBreadcrumbItem(isEditMode ? "Edit" : "New", null));
            }
        }

        private void ShowAddForm()
        {
            ClearExpressionCache();
            currentItem = new TEntity();
            isEditMode = false;
            showForm = true;
            UpdateBreadcrumb();
        }

        private void EditItem(TEntity item)
        {
            ClearExpressionCache();
            currentItem = CloneItem(item);
            isEditMode = true;
            showForm = true;
            UpdateBreadcrumb();
        }

        private async Task SaveItem()
        {
            if (editForm?.EditContext != null && !editForm.EditContext.Validate())
            {
                return;
            }

            await OnSave.InvokeAsync(currentItem);
            showForm = false;
            UpdateBreadcrumb();
            ToastService.ShowToast($"{Title} saved successfully!", ToastLevel.Success);
            StateHasChanged();
        }

        private void CancelForm()
        {
            ClearExpressionCache();
            showForm = false;
            currentItem = new TEntity();
            UpdateBreadcrumb();
        }

        private async Task DeleteItem(TEntity item)
        {
            var confirmed = await confirmDialog.Show($"Are you sure you want to delete this {Title}?");
            if (confirmed)
            {
                await OnDelete.InvokeAsync(item);
                ToastService.ShowToast($"{Title} deleted successfully.", ToastLevel.Success);
                StateHasChanged();
            }
        }

        public void OpenModal(TEntity item)
        {
            selectedModalItem = item;
            genericModal.Show();
            StateHasChanged();
        }

        private TEntity CloneItem(TEntity source)
        {
            var clone = new TEntity();
            foreach (var field in fieldMetadata)
            {
                var value = CrudMetadataHelper.GetPropertyValue(source, field.PropertyName);
                CrudMetadataHelper.SetPropertyValue(clone, field.PropertyName, value);
            }
            return clone;
        }

        private RenderFragment RenderFormField(CrudMetadataHelper.FieldMetadata field) => builder =>
        {
            var value = CrudMetadataHelper.GetPropertyValue(currentItem, field.PropertyName);
            var label = field.Label + (field.Required ? " *" : "");
            
            switch (field.FieldType?.ToLower() ?? "text")
            {
                case "text":
                case "email":
                case "password":
                    builder.OpenComponent<SfTextBox>(0);
                    builder.AddAttribute(1, "Value", value?.ToString() ?? string.Empty);
                    builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string>(this, newValue =>
                    {
                        CrudMetadataHelper.SetPropertyValue(currentItem, field.PropertyName, newValue);
                        editForm?.EditContext?.NotifyFieldChanged(new FieldIdentifier(currentItem, field.PropertyName));
                    }));
                    builder.AddAttribute(3, "Placeholder", field.Label);
                    builder.AddAttribute(4, "FloatLabelType", FloatLabelType.Auto);
                    builder.AddAttribute(5, "Readonly", field.ReadOnly);
                    builder.AddAttribute(6, "CssClass", "e-outline");
                    builder.AddAttribute(7, "ValidateOnInput", true);
                    if (field.FieldType?.ToLower() == "password")
                        builder.AddAttribute(8, "Type", InputType.Password);
                    builder.CloseComponent();
                    RenderValidationMessage(builder, field, 100);
                    break;

                case "number":
                    var propType = typeof(TEntity).GetProperty(field.PropertyName)?.PropertyType;
                    if (propType == typeof(int) || propType == typeof(int?))
                    {
                        RenderNumericField<int>(builder, value, field);
                    }
                    else if (propType == typeof(double) || propType == typeof(double?))
                    {
                        RenderNumericField<double>(builder, value, field);
                    }
                    else
                    {
                        RenderNumericField<decimal>(builder, value, field);
                    }
                    RenderValidationMessage(builder, field, 100);
                    break;

                case "checkbox":
                    builder.OpenComponent<SfCheckBox<bool>>(0);
                    builder.AddAttribute(1, "Checked", value != null && (bool)value);
                    builder.AddAttribute(2, "CheckedChanged", EventCallback.Factory.Create<bool>(this, newValue =>
                    {
                        CrudMetadataHelper.SetPropertyValue(currentItem, field.PropertyName, newValue);
                        editForm?.EditContext?.NotifyFieldChanged(new FieldIdentifier(currentItem, field.PropertyName));
                    }));
                    builder.AddAttribute(3, "Label", label);
                    builder.AddAttribute(4, "Disabled", field.ReadOnly);
                    builder.CloseComponent();
                    RenderValidationMessage(builder, field, 100);
                    break;

                case "date":
                case "datetime":
                    builder.OpenComponent<SfDatePicker<DateTime?>>(0);
                    builder.AddAttribute(1, "Value", value as DateTime?);
                    builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<DateTime?>(this, newValue =>
                    {
                        CrudMetadataHelper.SetPropertyValue(currentItem, field.PropertyName, newValue);
                        editForm?.EditContext?.NotifyFieldChanged(new FieldIdentifier(currentItem, field.PropertyName));
                    }));
                    builder.AddAttribute(3, "Placeholder", field.Label);
                    builder.AddAttribute(4, "FloatLabelType", FloatLabelType.Auto);
                    builder.AddAttribute(5, "Format", field.Format ?? "dd MMM yyyy");
                    builder.AddAttribute(6, "Readonly", field.ReadOnly);
                    builder.AddAttribute(7, "CssClass", "e-outline");
                    builder.CloseComponent();
                    RenderValidationMessage(builder, field, 100);
                    break;

                case "dropdown":
                    builder.OpenComponent<SfDropDownList<string, string>>(0);
                    builder.AddAttribute(1, "Value", value?.ToString() ?? string.Empty);
                    builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string>(this, newValue =>
                    {
                        CrudMetadataHelper.SetPropertyValue(currentItem, field.PropertyName, newValue);
                        editForm?.EditContext?.NotifyFieldChanged(new FieldIdentifier(currentItem, field.PropertyName));
                    }));
                    builder.AddAttribute(3, "Placeholder", field.Label);
                    builder.AddAttribute(4, "FloatLabelType", FloatLabelType.Auto);
                    builder.AddAttribute(5, "AllowFiltering", true);
                    builder.AddAttribute(6, "CssClass", "e-outline");
                    
                    List<string> dataSource = new();
                    if (DropdownDataSources != null && DropdownDataSources.ContainsKey(field.PropertyName))
                        dataSource = DropdownDataSources[field.PropertyName];
                    else if (!string.IsNullOrEmpty(field.DataSource))
                        dataSource = field.DataSource.Split(',').Select(s => s.Trim()).ToList();
                    
                    builder.AddAttribute(7, "DataSource", dataSource);
                    builder.AddAttribute(8, "Readonly", field.ReadOnly);
                    builder.CloseComponent();
                    RenderValidationMessage(builder, field, 100);
                    break;

                case "textarea":
                    builder.OpenComponent<SfTextBox>(0);
                    builder.AddAttribute(1, "Value", value?.ToString() ?? string.Empty);
                    builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string>(this, newValue =>
                    {
                        CrudMetadataHelper.SetPropertyValue(currentItem, field.PropertyName, newValue);
                        editForm?.EditContext?.NotifyFieldChanged(new FieldIdentifier(currentItem, field.PropertyName));
                    }));
                    builder.AddAttribute(3, "Placeholder", field.Label);
                    builder.AddAttribute(4, "FloatLabelType", FloatLabelType.Auto);
                    builder.AddAttribute(5, "Multiline", true);
                    builder.AddAttribute(6, "Readonly", field.ReadOnly);
                    builder.AddAttribute(7, "CssClass", "e-outline");
                    builder.AddAttribute(8, "ValidateOnInput", true);
                    builder.CloseComponent();
                    RenderValidationMessage(builder, field, 100);
                    break;

                default:
                    builder.OpenElement(0, "div");
                    builder.AddContent(1, $"Unsupported field type: {field.FieldType}");
                    builder.CloseElement();
                    break;
            }
        };

        private void RenderNumericField<T>(RenderTreeBuilder builder, object? value, CrudMetadataHelper.FieldMetadata field)
        {
            builder.OpenComponent<SfNumericTextBox<T>>(10);
            builder.AddAttribute(11, "Value", value != null ? (T)Convert.ChangeType(value, typeof(T)) : default(T));
            builder.AddAttribute(12, "ValueChanged", EventCallback.Factory.Create<T>(this, newValue =>
            {
                CrudMetadataHelper.SetPropertyValue(currentItem, field.PropertyName, newValue);
                editForm?.EditContext?.NotifyFieldChanged(new FieldIdentifier(currentItem, field.PropertyName));
            }));
            builder.AddAttribute(13, "Placeholder", field.Placeholder ?? field.Label);
            builder.AddAttribute(14, "FloatLabelType", FloatLabelType.Auto);
            builder.AddAttribute(15, "Format", field.Format ?? "N2");
            builder.AddAttribute(16, "Readonly", field.ReadOnly);
            builder.AddAttribute(17, "CssClass", "e-outline");
            builder.AddAttribute(18, "ValidateOnInput", true);
            builder.CloseComponent();
        }

        private void RenderValidationMessage(RenderTreeBuilder builder, CrudMetadataHelper.FieldMetadata field, int sequence)
        {
            if (editForm?.EditContext == null) return;

            var fieldIdentifier = new FieldIdentifier(currentItem, field.PropertyName);
            var messages = editForm.EditContext.GetValidationMessages(fieldIdentifier);

            if (messages.Any())
            {
                builder.OpenElement(sequence, "div");
                builder.AddAttribute(sequence + 1, "class", "validation-message");
                builder.AddContent(sequence + 2, messages.First());
                builder.CloseElement();
            }
        }

        private void ShowExportDialog(string format)
        {
            // Prepare column information for export dialog
            var availableColumns = fieldMetadata
                .Select(f => new ExportDialog.ColumnInfo
                {
                    PropertyName = f.PropertyName,
                    Label = f.Label,
                    IsSelected = f.ShowInGrid, // By default, select columns that are visible in grid
                    IsVisibleInGrid = f.ShowInGrid
                })
                .ToList();

            exportDialog.AvailableColumns = availableColumns;
            exportDialog.Show(format);
        }

        private int GetDisplayedRecordsCount()
        {
            if (grid?.PageSettings != null)
            {
                var pageSize = grid.PageSettings.PageSize;
                var currentPage = grid.PageSettings.CurrentPage;
                var totalRecords = DataSource.Count;
                
                var startIndex = (currentPage - 1) * pageSize;
                var endIndex = Math.Min(startIndex + pageSize, totalRecords);
                
                return endIndex - startIndex;
            }
            return DataSource.Count;
        }

        private async Task HandleExport(ExportDialog.ExportOptions options)
        {
            if (grid == null) return;

            List<TEntity> dataToExport;

            switch (options.Type)
            {
                case ExportDialog.ExportType.CurrentPage:
                    // Export only current page
                    if (grid.PageSettings != null)
                    {
                        var pageSize = grid.PageSettings.PageSize;
                        var currentPage = grid.PageSettings.CurrentPage;
                        var startIndex = (currentPage - 1) * pageSize;
                        dataToExport = DataSource.Skip(startIndex).Take(pageSize).ToList();
                    }
                    else
                    {
                        dataToExport = DataSource;
                    }
                    break;

                case ExportDialog.ExportType.CustomRange:
                    // Export custom range
                    var from = Math.Max(0, options.FromRecord - 1);
                    var count = Math.Min(options.ToRecord - from, DataSource.Count - from);
                    dataToExport = DataSource.Skip(from).Take(count).ToList();
                    break;

                case ExportDialog.ExportType.AllRecords:
                default:
                    // Export all records
                    dataToExport = DataSource;
                    break;
            }
            
            // Store original datasource (declare outside try for catch block access)
            var originalDataSource = DataSource;
            
            try
            {
                // Store original column visibility
                var originalColumnVisibility = new Dictionary<string, bool>();
                foreach (var field in fieldMetadata)
                {
                    originalColumnVisibility[field.PropertyName] = field.ShowInGrid;
                }

                // Update column visibility based on user selection
                if (options.SelectedColumns != null && options.SelectedColumns.Any())
                {
                    foreach (var field in fieldMetadata)
                    {
                        field.ShowInGrid = options.SelectedColumns.Contains(field.PropertyName);
                    }
                }
                
                // Temporarily update grid datasource for export
                DataSource = dataToExport;
                await InvokeAsync(StateHasChanged);
                await Task.Delay(200); // Give grid time to update columns and data

                if (options.Format == "excel")
                {
                    await grid.ExportToExcelAsync();
                }
                else if (options.Format == "pdf")
                {
                    await grid.ExportToPdfAsync();
                }

                // Restore original datasource and column visibility
                await Task.Delay(500); // Wait for export to complete
                DataSource = originalDataSource;
                
                // Restore original column visibility
                foreach (var field in fieldMetadata)
                {
                    if (originalColumnVisibility.ContainsKey(field.PropertyName))
                    {
                        field.ShowInGrid = originalColumnVisibility[field.PropertyName];
                    }
                }
                
                await InvokeAsync(StateHasChanged);

                var selectedColumnsCount = options.SelectedColumns?.Count ?? fieldMetadata.Count(f => f.ShowInGrid);
                ToastService.ShowToast($"Exported {dataToExport.Count} records with {selectedColumnsCount} columns successfully!", ToastLevel.Success);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Export failed: {ex.Message}", ToastLevel.Error);
                
                // Restore original state in case of error
                DataSource = originalDataSource;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
