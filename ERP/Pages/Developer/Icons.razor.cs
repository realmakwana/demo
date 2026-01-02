using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ERP.Components.Shared.UI;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace ERP.Pages.Developer
{
    public partial class Icons : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        
        private bool isLoading = true;
        private string searchTerm = "";
        private string? copiedIcon = null;
        private List<AppBreadcrumbItem> breadcrumbItems = new();
        private List<IconInfo> allIcons = new();
        private List<IconInfo> filteredIcons = new();

        protected override async Task OnInitializedAsync()
        {
            breadcrumbItems = new List<AppBreadcrumbItem>
            {
                new AppBreadcrumbItem("Home", "/"),
                new AppBreadcrumbItem("Developer Guide", null),
                new AppBreadcrumbItem("Icons", null)
            };

            await LoadIcons();
            filteredIcons = allIcons;
        }

        private void OnSearchChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? "";
            FilterIcons();
            StateHasChanged();
        }

        private void FilterIcons()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredIcons = allIcons;
            }
            else
            {
                var search = searchTerm.ToLower();
                filteredIcons = allIcons
                    .Where(i => i.Name.ToLower().Contains(search) || 
                               i.ClassName.ToLower().Contains(search))
                    .ToList();
            }
        }

        private async Task LoadIcons()
        {
            isLoading = true;
            StateHasChanged();

            // First load the manual list as fallback/initial
            var manualIcons = new List<IconInfo>
            {
                // A
                new IconInfo("Add", "bi bi-plus-lg"),
                // ... (Keep strictly necessary ones if fetch fails, but for now I will clear this or keep it minimal)
            };
            
            try 
            {
                using var client = new HttpClient();
                // Use a comprehensive list of Bootstrap Icons v1.11.3
                var cssContent = await client.GetStringAsync("https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css");
                
                var matches = Regex.Matches(cssContent, @"\.bi-([a-z0-9-]+)::before");
                
                allIcons = matches
                    .Select(m => m.Groups[1].Value)
                    .Distinct()
                    .OrderBy(name => name)
                    .Select(name => new IconInfo(ToTitleCase(name), $"bi bi-{name}"))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching icons: {ex.Message}");
                // Fallback to manual list if online fetch fails
                allIcons = GetManualIcons();
            }

            isLoading = false;
            FilterIcons();
            StateHasChanged();
        }

        private string ToTitleCase(string kebabCase)
        {
            if (string.IsNullOrEmpty(kebabCase)) return kebabCase;
            var words = kebabCase.Split('-');
            return string.Join(" ", words.Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w.Substring(1) : w));
        }

        private List<IconInfo> GetManualIcons()
        {
             return new List<IconInfo>
            {
                new IconInfo("Add", "bi bi-plus-lg"),
                new IconInfo("Add Circle", "bi bi-plus-circle"),
                new IconInfo("Add Square", "bi bi-plus-square"),
                new IconInfo("Alarm", "bi bi-alarm"),
                new IconInfo("Archive", "bi bi-archive"),
                new IconInfo("Arrow Down", "bi bi-arrow-down"),
                new IconInfo("Arrow Left", "bi bi-arrow-left"),
                new IconInfo("Arrow Right", "bi bi-arrow-right"),
                new IconInfo("Arrow Up", "bi bi-arrow-up"),
                new IconInfo("Asterisk", "bi bi-asterisk"),
                new IconInfo("Award", "bi bi-award"),
                new IconInfo("Briefcase", "bi bi-briefcase"),
                new IconInfo("Building", "bi bi-building"),
                new IconInfo("Calendar", "bi bi-calendar-event"),
                new IconInfo("Camera", "bi bi-camera"),
                new IconInfo("Check", "bi bi-check-lg"),
                new IconInfo("Check Circle", "bi bi-check-circle"),
                new IconInfo("Check Square", "bi bi-check-square"),
                new IconInfo("Clock", "bi bi-clock"),
                new IconInfo("Cloud", "bi bi-cloud"),
                new IconInfo("Credit Card", "bi bi-credit-card"),
                new IconInfo("Dashboard", "bi bi-speedometer2"),
                new IconInfo("Database", "bi bi-database"),
                new IconInfo("Delete", "bi bi-trash"),
                new IconInfo("Download", "bi bi-download"),
                new IconInfo("Edit", "bi bi-pencil"),
                new IconInfo("Envelope", "bi bi-envelope"),
                new IconInfo("Exclamation", "bi bi-exclamation-circle"),
                new IconInfo("Eye", "bi bi-eye"),
                new IconInfo("Eye Slash", "bi bi-eye-slash"),
                new IconInfo("File", "bi bi-file-earmark"),
                new IconInfo("File Excel", "bi bi-file-earmark-excel"),
                new IconInfo("File PDF", "bi bi-file-earmark-pdf"),
                new IconInfo("Gear", "bi bi-gear"),
                new IconInfo("House", "bi bi-house"),
                new IconInfo("Info", "bi bi-info-circle"),
                new IconInfo("Lock", "bi bi-lock"),
                new IconInfo("Menu", "bi bi-list"),
                new IconInfo("Person", "bi bi-person"),
                new IconInfo("Phone", "bi bi-phone"),
                new IconInfo("Printer", "bi bi-printer"),
                new IconInfo("Save", "bi bi-save"),
                new IconInfo("Search", "bi bi-search"),
                new IconInfo("Settings", "bi bi-gear-fill"),
                new IconInfo("Truck", "bi bi-truck"),
                new IconInfo("Upload", "bi bi-upload"),
                new IconInfo("User", "bi bi-person-circle"),
                new IconInfo("Wifi", "bi bi-wifi"),
            };
        }

        private async Task CopyIconClass(string className)
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", className);
            copiedIcon = className;
            StateHasChanged();
            
            // Reset after 2 seconds
            await Task.Delay(2000);
            copiedIcon = null;
            StateHasChanged();
        }

        public class IconInfo
        {
            public string Name { get; set; }
            public string ClassName { get; set; }

            public IconInfo(string name, string className)
            {
                Name = name;
                ClassName = className;
            }
        }
    }
}
