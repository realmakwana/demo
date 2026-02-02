using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ERP.Components.Base
{
    /// <summary>
    /// Base component for CRUD pages with keyboard shortcuts support
    /// Provides Alt+S for Save and Alt+F for Search functionality
    /// </summary>
    public abstract class CrudComponentBase : ComponentBase, IAsyncDisposable
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

        private DotNetObjectReference<CrudComponentBase>? dotNetHelper;
        protected bool isKeyboardInitialized = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeKeyboardShortcuts();
            }
        }

        /// <summary>
        /// Initialize keyboard shortcuts module
        /// </summary>
        private async Task InitializeKeyboardShortcuts()
        {
            try
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                
                // Call the global JavaScript function directly (not as module)
                await JSRuntime.InvokeVoidAsync("keyboardShortcuts.initialize", dotNetHelper);
                
                isKeyboardInitialized = true;
                Console.WriteLine("Keyboard shortcuts initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing keyboard shortcuts: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when Alt+S is pressed
        /// Override this method in derived classes to implement save functionality
        /// </summary>
        [JSInvokable]
        public virtual async Task HandleSaveShortcut()
        {
            Console.WriteLine("HandleSaveShortcut called - Override this method in derived class");
            await OnSaveShortcut();
        }

        /// <summary>
        /// Called when Alt+F is pressed
        /// Override this method in derived classes to implement search focus functionality
        /// </summary>
        [JSInvokable]
        public virtual async Task HandleSearchShortcut()
        {
            Console.WriteLine("HandleSearchShortcut called - Override this method in derived class");
            await OnSearchShortcut();
        }

        /// <summary>
        /// Override this method to handle save shortcut
        /// </summary>
        protected virtual Task OnSaveShortcut()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to handle search shortcut
        /// </summary>
        protected virtual Task OnSearchShortcut()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Focus on search input using JavaScript
        /// </summary>
        protected async Task FocusSearchInput()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "keyboardShortcuts.focusElement",
                    ".e-search input, input[placeholder*='Search']");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error focusing search input: {ex.Message}");
            }
        }

        /// <summary>
        /// Trigger save button click using JavaScript
        /// </summary>
        protected async Task TriggerSaveButton()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "keyboardShortcuts.clickElement",
                    "button[type='submit']");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error triggering save button: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (isKeyboardInitialized)
                {
                    await JSRuntime.InvokeVoidAsync("keyboardShortcuts.dispose");
                }

                dotNetHelper?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing keyboard shortcuts: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when Alt+A is pressed (Add New Record)
        /// Override this in derived pages to open Add form
        /// </summary>
        [JSInvokable]
        public virtual async Task HandleAddShortcut()
        {
            await OnAddShortcut();
        }

        /// <summary>
        /// Override this to implement Add shortcut behavior
        /// </summary>
        protected virtual Task OnAddShortcut()
        {
            return Task.CompletedTask;
        }
    }
}
