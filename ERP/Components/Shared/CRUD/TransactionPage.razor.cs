using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Grids;
using ERP.Models.Entities;
using ERP.Components.Shared.UI;
using ERP.Models.Services;

namespace ERP.Components.Shared.CRUD
{
    public partial class TransactionPage<THeader, TLineItem> : ComponentBase
        where THeader : class, new()
        where TLineItem : class, new()
    {
        // Parameters
        [Parameter] public string Title { get; set; } = "Document";
        [Parameter] public string Subtitle { get; set; } = "";
        [Parameter] public string GridTitle { get; set; } = "All Documents";
        [Parameter] public string? HelpText { get; set; }
        [Parameter] public string LineItemsTitle { get; set; } = "Line Items";
        
        [Parameter] public List<THeader> DataSource { get; set; } = new();
        [Parameter] public EventCallback<(THeader header, List<TLineItem> items)> OnSave { get; set; }
        [Parameter] public EventCallback<THeader> OnDelete { get; set; }
        
        [Parameter] public RenderFragment? GridColumns { get; set; }
        [Parameter] public RenderFragment? HeaderFormContent { get; set; }
        [Parameter] public RenderFragment? LineItemFormContent { get; set; }
        [Parameter] public RenderFragment? LineItemGridColumns { get; set; }
        [Parameter] public RenderFragment? TotalsContent { get; set; }
        
        [Parameter] public EventCallback<TLineItem> OnLineItemAdded { get; set; }
        [Parameter] public EventCallback<TLineItem> OnLineItemRemoved { get; set; }
        [Parameter] public Func<THeader, List<TLineItem>>? GetLineItems { get; set; }
        [Parameter] public int TotalRecordsInDatabase { get; set; } = 0;

        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // State
        private SfGrid<THeader>? grid;
        private ConfirmationDialog confirmDialog = default!;
        private ExportDialog exportDialog = default!;
        private EditForm? headerForm;
        private THeader currentHeader = new();
        private List<TLineItem> currentLineItems = new();
        private bool showForm = false;
        private bool isEditMode = false;
        private bool isLoading = false;
        private List<AppBreadcrumbItem> breadcrumbItems = new();
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

        private void OnGridActionBegin(Syncfusion.Blazor.Grids.ActionEventArgs<THeader> args)
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

        private void OnGridActionComplete(Syncfusion.Blazor.Grids.ActionEventArgs<THeader> args)
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

        private void ShowAddForm()
        {
            currentHeader = new THeader();
            currentLineItems = new List<TLineItem>();
            isEditMode = false;
            showForm = true;
            UpdateBreadcrumb();
        }

        private void EditItem(THeader item)
        {
            currentHeader = CloneHeader(item);
            currentLineItems = GetLineItems?.Invoke(item)?.ToList() ?? new List<TLineItem>();
            isEditMode = true;
            showForm = true;
            UpdateBreadcrumb();
        }

        private async Task SaveItem()
        {
            if (headerForm?.EditContext != null && !headerForm.EditContext.Validate())
            {
                ToastService.ShowToast("Please fix validation errors in the header.", ToastLevel.Warning);
                return;
            }

            await OnSave.InvokeAsync((currentHeader, currentLineItems));
            showForm = false;
            UpdateBreadcrumb();
            ToastService.ShowToast($"{Title} saved successfully!", ToastLevel.Success);
            StateHasChanged();
        }

        private void CancelForm()
        {
            showForm = false;
            currentHeader = new THeader();
            currentLineItems = new List<TLineItem>();
            UpdateBreadcrumb();
        }

        private async Task DeleteItem(THeader item)
        {
            var confirmed = await confirmDialog.Show($"Are you sure you want to delete this {Title}?");
            if (confirmed)
            {
                await OnDelete.InvokeAsync(item);
                ToastService.ShowToast($"{Title} deleted successfully.", ToastLevel.Success);
                StateHasChanged();
            }
        }

        private async Task AddLineItem()
        {
            var newItem = new TLineItem();
            await OnLineItemAdded.InvokeAsync(newItem);
        }

        private async Task RemoveLineItem(TLineItem item)
        {
            var confirmed = await confirmDialog.Show("Remove this item from the list?");
            if (confirmed)
            {
                currentLineItems.Remove(item);
                await OnLineItemRemoved.InvokeAsync(item);
                ToastService.ShowToast("Item removed from list.", ToastLevel.Info);
                StateHasChanged();
            }
        }

        private THeader CloneHeader(THeader source)
        {
            var clone = new THeader();
            foreach (var prop in typeof(THeader).GetProperties())
            {
                if (prop.CanWrite && prop.CanRead)
                {
                    prop.SetValue(clone, prop.GetValue(source));
                }
            }
            return clone;
        }

        public void AddItemToList(TLineItem item)
        {
            currentLineItems.Add(item);
            StateHasChanged();
        }

        public List<TLineItem> GetCurrentLineItems() => currentLineItems;
        public THeader GetCurrentHeader() => currentHeader;

        private void ShowExportDialog(string format)
        {
            exportDialog.Show(format);  // Pass format to dialog
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

            List<THeader> dataToExport;
            int actualTotalRecords = TotalRecordsInDatabase > 0 ? TotalRecordsInDatabase : DataSource.Count;

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
                    // For demo purposes with dummy data:
                    // If user wants records beyond what's in DataSource, we'll generate dummy records
                    var from = Math.Max(0, options.FromRecord - 1);
                    var to = options.ToRecord;
                    
                    if (to > DataSource.Count)
                    {
                        // Generate additional dummy records for demonstration
                        dataToExport = new List<THeader>();
                        
                        // Add existing records in range
                        var existingInRange = DataSource.Skip(from).Take(Math.Min(DataSource.Count - from, to - from)).ToList();
                        dataToExport.AddRange(existingInRange);
                        
                        // Generate dummy records for the rest
                        var dummyCount = to - Math.Max(from, DataSource.Count);
                        for (int i = 0; i < dummyCount; i++)
                        {
                            var dummyRecord = new THeader();
                            // Set some dummy values using reflection
                            foreach (var prop in typeof(THeader).GetProperties())
                            {
                                if (prop.CanWrite)
                                {
                                    if (prop.PropertyType == typeof(string))
                                        prop.SetValue(dummyRecord, $"Dummy {prop.Name} {DataSource.Count + i + 1}");
                                    else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                                        prop.SetValue(dummyRecord, DataSource.Count + i + 1);
                                    else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                                        prop.SetValue(dummyRecord, DateTime.Now.AddDays(i));
                                    else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                                        prop.SetValue(dummyRecord, (decimal)(100 + i));
                                }
                            }
                            dataToExport.Add(dummyRecord);
                        }
                    }
                    else
                    {
                        var count = Math.Min(to - from, DataSource.Count - from);
                        dataToExport = DataSource.Skip(from).Take(count).ToList();
                    }
                    break;

                case ExportDialog.ExportType.AllRecords:
                default:
                    // Export all records
                    // If TotalRecordsInDatabase is set and greater than DataSource, generate dummy data
                    if (actualTotalRecords > DataSource.Count)
                    {
                        dataToExport = new List<THeader>(DataSource);
                        var dummyCount = actualTotalRecords - DataSource.Count;
                        
                        for (int i = 0; i < dummyCount; i++)
                        {
                            var dummyRecord = new THeader();
                            foreach (var prop in typeof(THeader).GetProperties())
                            {
                                if (prop.CanWrite)
                                {
                                    if (prop.PropertyType == typeof(string))
                                        prop.SetValue(dummyRecord, $"Dummy {prop.Name} {DataSource.Count + i + 1}");
                                    else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                                        prop.SetValue(dummyRecord, DataSource.Count + i + 1);
                                    else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                                        prop.SetValue(dummyRecord, DateTime.Now.AddDays(i));
                                    else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                                        prop.SetValue(dummyRecord, (decimal)(100 + i));
                                }
                            }
                            dataToExport.Add(dummyRecord);
                        }
                    }
                    else
                    {
                        dataToExport = DataSource;
                    }
                    break;
            }
            
            try
            {
                // Store original datasource
                var originalDataSource = DataSource;
                
                // Temporarily update grid datasource for export
                DataSource = dataToExport;
                await InvokeAsync(StateHasChanged);
                await Task.Delay(100); // Give grid time to update

                if (options.Format == "excel")
                {
                    await grid.ExportToExcelAsync();
                }
                else if (options.Format == "pdf")
                {
                    await grid.ExportToPdfAsync();
                }

                // Restore original datasource
                await Task.Delay(500); // Wait for export to complete
                DataSource = originalDataSource;
                await InvokeAsync(StateHasChanged);

                ToastService.ShowToast($"Exported {dataToExport.Count} records successfully!", ToastLevel.Success);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Export failed: {ex.Message}", ToastLevel.Error);
            }
        }
    }
}
