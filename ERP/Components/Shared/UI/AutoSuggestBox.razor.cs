using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Inputs;
using ERP.Models.Entities;

namespace ERP.Components.Shared.UI
{
    public partial class AutoSuggestBox : ComponentBase
    {
        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public FloatLabelType FloatLabelType { get; set; } = FloatLabelType.Auto;

        [Parameter]
        public string? CssClass { get; set; }

        [Parameter]
        public Func<string, Task<IEnumerable<string>>>? SearchFunction { get; set; }

        [Parameter]
        public Func<string, Task<IEnumerable<string>>>? SearchStudentFunction { get; set; }


        private IEnumerable<string> _currentData = Enumerable.Empty<string>();
        private SfAutoComplete<string, string>? AutoCompleteObj;

        private async Task HandleValueChanged(string newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(newValue);
        }

        private async Task OnFiltering(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (string.IsNullOrEmpty(args.Text) || args.Text.Length < 3)
            {
                if (AutoCompleteObj != null) await AutoCompleteObj.HidePopupAsync();
                return;
            }

            if (SearchFunction != null && AutoCompleteObj != null)
            {
                var results = await SearchFunction(args.Text);
                await AutoCompleteObj.FilterAsync(results, new Query());
            }
        }
    }
}
