namespace ERP.Components.Shared.UI
{
    public class AppBreadcrumbItem
    {
        public string Label { get; set; } = string.Empty;
        public string? Url { get; set; }

        public AppBreadcrumbItem() { }

        public AppBreadcrumbItem(string label, string? url = null)
        {
            Label = label;
            Url = url;
        }
    }
}
