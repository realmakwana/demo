using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ERP.Components.Shared.UI;

namespace ERP.Pages
{
    public partial class Settings : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new AppBreadcrumbItem("Home", "/"),
            new AppBreadcrumbItem("Settings", null)
        };

        private string selectedLanguage = "English (US)";
        private List<string> languages = new() { "English (US)", "Hindi", "Gujarati", "Marathi", "Spanish" };

        private string selectedTimezone = "(UTC+05:30) Mumbai, Kolkata, New Delhi";
        private List<string> timezones = new() { "(UTC+05:30) Mumbai, Kolkata, New Delhi", "(UTC+00:00) London", "(UTC-05:00) New York" };

        private bool autoSave = true;
        private bool notifInvoice = true;
        private bool notifSystem = false;
        private bool glassSidebar = false;
        private int animSpeed = 300;

        private string selectedThemeColor = "#2563eb";
        private List<ColorOption> themeColors = new()
        {
            new ColorOption { Name = "Blue", Value = "#2563eb" },
            new ColorOption { Name = "Purple", Value = "#7c3aed" },
            new ColorOption { Name = "Pink", Value = "#ec4899" },
            new ColorOption { Name = "Red", Value = "#ef4444" },
            new ColorOption { Name = "Orange", Value = "#f97316" },
            new ColorOption { Name = "Green", Value = "#10b981" },
            new ColorOption { Name = "Teal", Value = "#14b8a6" },
            new ColorOption { Name = "Indigo", Value = "#6366f1" }
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                selectedThemeColor = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "theme-color") ?? "#2563eb";
            }
            catch
            {
                selectedThemeColor = "#2563eb";
            }
        }

        private void SelectThemeColor(string color)
        {
            selectedThemeColor = color;
        }

        private async Task ApplySettings()
        {
            await JSRuntime.InvokeVoidAsync("setThemeColor", selectedThemeColor);
            // Additional settings persistence logic can go here (e.g., saving to DB or other localStorage keys)
        }

        private class ColorOption
        {
            public string Name { get; set; } = "";
            public string Value { get; set; } = "";
        }
    }
}
