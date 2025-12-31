using Microsoft.AspNetCore.Components;
using ERP.Components.Shared.UI;
using Syncfusion.Blazor.Grids;

namespace ERP.Pages
{
    public partial class SyncfusionControlsDemo : ComponentBase
    {
        private List<AppBreadcrumbItem> breadcrumbItems = new();

        protected override void OnInitialized()
        {
            breadcrumbItems = new List<AppBreadcrumbItem>
            {
                new AppBreadcrumbItem("Home", "/"),
                new AppBreadcrumbItem("Syncfusion Controls", null)
            };
        }

        private string textValue = "";
        private int? numericValue;
        private string phoneValue = "";
        private DateTime? dateValue;
        private DateTime? dateTimeValue;
        private DateTime? timeValue;
        private string selectedDropdown = "";
        private string selectedCombo = "";
        private string[] selectedMultiple = Array.Empty<string>();
        private bool checkboxValue = false;
        private bool switchValue = false;
        private int sliderValue = 50;
        private string colorValue = "#2563eb";
        private string message = "";
        
        private List<string> dropdownData = new List<string>
        {
            "Option 1", "Option 2", "Option 3", "Option 4", "Option 5"
        };

        private void ShowMessage()
        {
            message = $"Button clicked at {DateTime.Now:HH:mm:ss}";
            StateHasChanged();
        }
        
        public class Order
        {
            public int? OrderID { get; set; }
            public string? CustomerID { get; set; }
            public DateTime? OrderDate { get; set; }
            public double? Freight { get; set; }
        }

        public List<Order> gridData = new List<Order>
        {
            new Order { OrderID = 10248, CustomerID = "VINET", OrderDate = new DateTime(1996, 7, 4), Freight = 32.38 },
            new Order { OrderID = 10249, CustomerID = "TOMSP", OrderDate = new DateTime(1996, 7, 5), Freight = 11.61 },
            new Order { OrderID = 10250, CustomerID = "HANAR", OrderDate = new DateTime(1996, 7, 8), Freight = 65.83 },
            new Order { OrderID = 10251, CustomerID = "VICTE", OrderDate = new DateTime(1996, 7, 8), Freight = 41.34 },
            new Order { OrderID = 10252, CustomerID = "SUPRD", OrderDate = new DateTime(1996, 7, 9), Freight = 51.30 }
        };
    }
}
