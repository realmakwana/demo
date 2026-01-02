namespace ERP.Models.Helpers
{
    public static class IconHelper
    {
        // Syncfusion Icon Classes - Constants at the top
        private const string DefaultIcon = "e-icons e-circle";
        private const string DashboardIcon = "e-icons e-dashboard";
        private const string UserIcon = "e-icons e-user";
        private const string SettingsIcon = "e-icons e-settings";
        private const string CodeIcon = "e-icons e-code";
        private const string PaletteIcon = "e-icons e-palette";
        private const string EmojiIcon = "e-icons e-emoji";

        /// <summary>
        /// Returns Syncfusion icon class based on className from database
        /// If className starts with "e-icons", it's returned as-is
        /// Otherwise, keyword mapping is used
        /// </summary>
        public static string GetIcon(string? className)
        {
            if (string.IsNullOrEmpty(className)) return "bi bi-circle";

            var trimmed = className.Trim();

            // If it's already a Bootstrap icon class, return it
            if (trimmed.StartsWith("bi-", StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith("bi ", StringComparison.OrdinalIgnoreCase))
                return trimmed;

            // Legacy Syncfusion support: Try to Map Syncfusion classes to Bootstrap
            if (trimmed.StartsWith("e-icons", StringComparison.OrdinalIgnoreCase))
            {
                var sfIcon = trimmed.Replace("e-icons e-", "").Trim();
                return GetBootstrapIconForSyncfusion(sfIcon);
            }

            // Keyword mapping
            var lower = trimmed.ToLower();
            return GetBootstrapIconForKeyword(lower);
        }

        private static string GetBootstrapIconForSyncfusion(string sfIcon)
        {
            return sfIcon switch
            {
                "home" => "bi bi-house-door",
                "layers" => "bi bi-layers",
                "group" => "bi bi-people",
                "pepole" => "bi bi-person", // Typo in DB
                "people" => "bi bi-people-fill",
                "office" => "bi bi-building",
                "car" => "bi bi-truck",
                "contact" => "bi bi-person-lines-fill",
                "changes-accept" => "bi bi-arrow-left-right",
                "thumbnail" => "bi bi-receipt",
                "calendar" => "bi bi-calendar-event",
                "money" => "bi bi-cash-stack",
                "chart" => "bi bi-bar-chart",
                "settings" => "bi bi-gear",
                "gear" => "bi bi-sliders",
                "palette" => "bi bi-palette",
                "lock" => "bi bi-shield-lock",
                "insert-code" => "bi bi-code-square",
                "emoji" => "bi bi-emoji-smile",
                "shopping" => "bi bi-bag",
                "cart" => "bi bi-cart",
                "dollar" => "bi bi-currency-dollar",
                // Defaults
                "dashboard" => "bi bi-speedometer2",
                "user" => "bi bi-person",
                "code" => "bi bi-code-slash",
                "file-text" => "bi bi-file-text",
                "folder" => "bi bi-folder",
                "layout" => "bi bi-layout-text-window-reverse",
                _ => $"bi bi-{sfIcon}" // Fallback (might not work for all)
            };
        }

        private static string GetBootstrapIconForKeyword(string keyword)
        {
            if (keyword.Contains("dashboard")) return "bi bi-speedometer2";
            if (keyword.Contains("user") || keyword.Contains("profile")) return "bi bi-person";
            if (keyword.Contains("setting") || keyword.Contains("gear")) return "bi bi-gear";
            if (keyword.Contains("code")) return "bi bi-code-slash";
            if (keyword.Contains("palette")) return "bi bi-palette";
            if (keyword.Contains("master")) return "bi bi-grid-fill";
            if (keyword.Contains("company") || keyword.Contains("office")) return "bi bi-building";
            if (keyword.Contains("vehicle") || keyword.Contains("car") || keyword.Contains("truck")) return "bi bi-truck";
            if (keyword.Contains("driver")) return "bi bi-person-badge";
            if (keyword.Contains("report") || keyword.Contains("chart")) return "bi bi-graph-up";
            if (keyword.Contains("invoice")) return "bi bi-receipt";
            
            return "bi bi-circle";
        }
    }
}
