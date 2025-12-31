using System;

namespace TransportERP.Models.Attributes
{
    /// <summary>
    /// Attribute to configure how a field should be displayed in CRUD forms
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CrudFieldAttribute : Attribute
    {
        /// <summary>
        /// Display label for the field
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Field type: Text, Number, Dropdown, Date, DateTime, Checkbox, TextArea
        /// </summary>
        public string FieldType { get; set; } = "Text";

        /// <summary>
        /// Is this field required?
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// Display order in form (lower numbers appear first)
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Show in grid?
        /// </summary>
        public bool ShowInGrid { get; set; } = true;

        /// <summary>
        /// Grid column width
        /// </summary>
        public string GridWidth { get; set; } = "150";

        /// <summary>
        /// For dropdown fields - comma separated values or method name
        /// </summary>
        public string DataSource { get; set; } = string.Empty;

        /// <summary>
        /// Number format (for numeric fields): N0, N2, C2, etc.
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// Placeholder text
        /// </summary>
        public string Placeholder { get; set; } = string.Empty;

        /// <summary>
        /// Is this field read-only?
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Hide this field from form?
        /// </summary>
        public bool HideInForm { get; set; } = false;

        /// <summary>
        /// Custom template name for grid column
        /// </summary>
        public string GridTemplate { get; set; } = string.Empty;
    }
}
