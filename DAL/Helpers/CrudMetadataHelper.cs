using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TransportERP.Models.Attributes;

namespace TransportERP.Models.Helpers
{
    /// <summary>
    /// Helper class to extract CRUD metadata from entity types
    /// </summary>
    public class CrudMetadataHelper
    {
        public class FieldMetadata
        {
            public string PropertyName { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public string FieldType { get; set; } = "Text";
            public bool Required { get; set; }
            public int Order { get; set; }
            public bool ShowInGrid { get; set; }
            public string GridWidth { get; set; } = "150";
            public string DataSource { get; set; } = string.Empty;
            public string Format { get; set; } = string.Empty;
            public string Placeholder { get; set; } = string.Empty;
            public bool ReadOnly { get; set; }
            public bool HideInForm { get; set; }
            public string GridTemplate { get; set; } = string.Empty;
            public PropertyInfo PropertyInfo { get; set; } = null!;
            public Type PropertyType { get; set; } = null!;
        }

        public static List<FieldMetadata> GetFieldMetadata<T>() where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fieldMetadataList = new List<FieldMetadata>();

            foreach (var prop in properties)
            {
                var crudAttr = prop.GetCustomAttribute<CrudFieldAttribute>();
                
                if (crudAttr != null)
                {
                    fieldMetadataList.Add(new FieldMetadata
                    {
                        PropertyName = prop.Name,
                        Label = string.IsNullOrEmpty(crudAttr.Label) ? prop.Name : crudAttr.Label,
                        FieldType = crudAttr.FieldType,
                        Required = crudAttr.Required,
                        Order = crudAttr.Order,
                        ShowInGrid = crudAttr.ShowInGrid,
                        GridWidth = crudAttr.GridWidth,
                        DataSource = crudAttr.DataSource,
                        Format = crudAttr.Format,
                        Placeholder = crudAttr.Placeholder,
                        ReadOnly = crudAttr.ReadOnly,
                        HideInForm = crudAttr.HideInForm,
                        GridTemplate = crudAttr.GridTemplate,
                        PropertyInfo = prop,
                        PropertyType = prop.PropertyType
                    });
                }
                else
                {
                    // Auto-detect field type if no attribute
                    var fieldType = GetDefaultFieldType(prop.PropertyType);
                    
                    fieldMetadataList.Add(new FieldMetadata
                    {
                        PropertyName = prop.Name,
                        Label = prop.Name,
                        FieldType = fieldType,
                        Required = false,
                        Order = 100,
                        ShowInGrid = !IsSystemField(prop.Name),
                        GridWidth = "150",
                        PropertyInfo = prop,
                        PropertyType = prop.PropertyType,
                        HideInForm = IsSystemField(prop.Name)
                    });
                }
            }

            return fieldMetadataList.OrderBy(f => f.Order).ThenBy(f => f.PropertyName).ToList();
        }

        private static string GetDefaultFieldType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (underlyingType == typeof(bool))
                return "Checkbox";
            if (underlyingType == typeof(DateTime))
                return "DateTime";
            if (underlyingType == typeof(int) || underlyingType == typeof(long) || 
                underlyingType == typeof(decimal) || underlyingType == typeof(double) || 
                underlyingType == typeof(float))
                return "Number";
            
            return "Text";
        }

        private static bool IsSystemField(string propertyName)
        {
            var systemFields = new[] { "Id", "CreatedDate", "CreatedBy", "ModifiedDate", "ModifiedBy" };
            return systemFields.Contains(propertyName);
        }

        public static object? GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

        public static void SetPropertyValue(object obj, string propertyName, object? value)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop != null && prop.CanWrite)
            {
                // Handle nullable types
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                
                if (value == null)
                {
                    prop.SetValue(obj, null);
                }
                else if (value.GetType() == targetType || value.GetType() == prop.PropertyType)
                {
                    prop.SetValue(obj, value);
                }
                else
                {
                    try
                    {
                        var convertedValue = Convert.ChangeType(value, targetType);
                        prop.SetValue(obj, convertedValue);
                    }
                    catch
                    {
                        // Conversion failed, skip
                    }
                }
            }
        }
    }
}
