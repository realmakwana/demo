# TransportERP Developer Manual

This comprehensive guide covers the architecture of the **TransportERP** project, how to add new modules (CRUD), and how to efficiently use the built-in features.

---

## 1. Project Overview

**TransportERP** is a Blazor Server application designed for transport management. It uses:
-   **Frontend**: Blazor Server with Syncfusion Components.
-   **Backend**: C# .NET 7/8.
-   **Database**: SQL Server (accessed via Dapper/ADO.NET stored procedures or Entity Framework).
-   **Styling**: Custom CSS variables for theming, responsive layout.

### Key Directories
-   `TransportERP.Server`: The main web application.
    -   `Pages`: Standard Blazor pages (Views).
    -   `Components/Shared/CRUD`: Core reusable CRUD components (`MasterPage`, `TransactionPage`).
    -   `Components/Shared/UI`: Reusable UI elements (`ExportDialog`, `ConfirmationDialog`).
-   `TransportERP.Models`: Shared class libraries.
    -   `Entities`: Database models/POCOs.
    -   `Services`: Business logic and data access layers.

---

## 2. Using the Software (User Guide)

Before diving into code, here is how the standardized interface works.

### Navigation & Layout
-   **Sidebar**: Contains the main menu. The menu is dynamic based on user rights.
-   **Breadcrumbs**: Located at the top of every page, allowing easy navigation back to "Home".
-   **Theme Settings**: Use the palette icon in the top right to change the application's primary color.

### working with Grids (Lists)
Every standard list page (like Drivers, Cities) supports:
-   **Sorting**: Click column headers.
-   **Filtering**: Use the filter bar button or type in the search box (if enabled).
-   **Paging**: Navigate through large datasets using the pagination controls at the bottom.
-   **Exporting**: Click the "Excel" or "PDF" buttons in the toolbar. You can choose to export:
    -   *Current Page*
    -   *All Records*
    -   *Custom Range* (e.g., Records 1 to 500).

### Forms
-   **Validation**: Fields in red are required or have invalid input.
-   **Components**: Standard inputs include Text Boxes, specialized Number inputs, Date Pickers, and Searchable Dropdowns.

---

## 3. How to Add a New Simple CRUD (Master Data)

Use the **`MasterPage<T>`** component to quickly create "Master" data management screens (e.g., Cities, Departments, Item Categories).

### Step 1: Create the Entity
Create a new class in `TransportERP.Models/Entities`. Inherit from `BaseEntity` if applicable.

**Example: `Department.cs`**
```csharp
using System.ComponentModel.DataAnnotations;
using TransportERP.Models.Attributes;

namespace TransportERP.Models.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        [CrudField(Label = "Department Name", Order = 1, ShowInGrid = true)]
        public string DepartmentName { get; set; } = "";

        [CrudField(Label = "Active Status", Order = 2, FieldType = "Checkbox")]
        public bool IsActive { get; set; } = true;
    }
}
```

#### Using Attributes for UI Customization
The `MasterPage` uses `[CrudField]` to control the UI:
-   **Label**: The display name in the form and grid.
-   **Order**: The order of fields in the form.
-   **ShowInGrid**: Whether to show this column in the grid.
-   **FieldType**: "Text", "Number", "Date", "Checkbox", "Dropdown", "TextArea".
-   **HideInForm**: Set to `true` to hide generic fields.
    *   *Note: System fields like `Id`, `CreatedDate`, `CreatedBy` are hidden by default.*

### Step 2: Create the Service
Create a service class (and interface) in `TransportERP.Models/Services` to handle data operations. The project uses `IDbContextFactory<ApplicationDbContext>` for handling EF Core contexts safely in Blazor Server.

**Example: `DepartmentService.cs`**
```csharp
using Microsoft.EntityFrameworkCore;
using TransportERP.Models.DbContext;
using TransportERP.Models.Entities;

namespace TransportERP.Models.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();
        Task<Department> SaveAsync(Department item);
        Task DeleteAsync(int id);
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DepartmentService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Departments.ToListAsync();
        }

        public async Task<Department> SaveAsync(Department item)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (item.DepartmentId == 0)
            {
                context.Departments.Add(item);
            }
            else
            {
                context.Departments.Update(item);
            }
            await context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var item = await context.Departments.FindAsync(id);
            if (item != null)
            {
                context.Departments.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
```

*Note: Ensure you register this service in `Program.cs` or `Startup.cs`:*
```csharp
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
```

### Step 3: Create the Razor Page
Create a new `.razor` file in `TransportERP.Server/Pages`. Inherit from `ComponentBase` or just use the markup.

**Example: `Departments.razor`**
```razor
@page "/departments"
@using ERP.Components.Shared.CRUD
@using TransportERP.Models.Entities

<MasterPage TEntity="Department"
            Title="Departments"
            Subtitle="Manage company departments"
            DataSource="@departments"
            OnSave="SaveDepartment"
            OnDelete="DeleteDepartment">
</MasterPage>

@code {
    [Inject] public IDepartmentService DepartmentService { get; set; }
    
    private List<Department> departments = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        departments = await DepartmentService.GetAllAsync();
    }

    private async Task SaveDepartment(Department item)
    {
        await DepartmentService.SaveAsync(item);
        await LoadData(); // Refresh grid
    }

    private async Task DeleteDepartment(Department item)
    {
        await DepartmentService.DeleteAsync(item.DepartmentId);
        await LoadData();
    }
}
```

### Step 4: Register in Menu
1.  Add the new menu item to your database (Table: `Menus`).
2.  Use the SQL script `Database/11_Complete_Menu_Icons_Update.sql` as a template to insert the new menu item with the URL `/departments`.

---

## 4. How to Add a New Transaction CRUD (Master-Detail)

Use the **`TransactionPage<THeader, TLineItem>`** component for complex documents like Invoices, Orders, or Challans.

### Concepts
-   **Header**: The main document info (Date, Customer, Total).
-   **Line Items**: The list of items within the document.

### Implementation Steps

1.  **Define Models**: You need two classes, e.g., `Order` (Header) and `OrderItem` (LineItem).
2.  **Create Page**: Create a new `.razor` file.

**Example Structure:**

```razor
@page "/orders"
@using ERP.Components.Shared.CRUD

<TransactionPage THeader="Order" TLineItem="OrderItem"
                 Title="Sales Orders"
                 DataSource="@orders"
                 OnSave="SaveOrder"
                 OnDelete="DeleteOrder"
                 OnLineItemAdded="AddItem"
                 OnLineItemRemoved="RemoveItem"
                 GetLineItems="GetLineItemsForOrder">

    @* Custom Grid Columns for the Main List *@
    <GridColumns>
        <GridColumn Field="@nameof(Order.OrderNo)" HeaderText="Order #" Width="100"></GridColumn>
        <GridColumn Field="@nameof(Order.OrderDate)" HeaderText="Date" Format="d"></GridColumn>
        <GridColumn Field="@nameof(Order.CustomerName)" HeaderText="Customer"></GridColumn>
    </GridColumns>

    @* Form Content for the Header *@
    <HeaderFormContent>
        <div class="row">
            <div class="col-md-6">
                <!-- Bind to context.OrderNo, etc. -->
            </div>
        </div>
    </HeaderFormContent>

    @* Form Content for Adding a Line Item *@
    <LineItemFormContent>
        <!-- Inputs for Product, Qty, Rate -->
    </LineItemFormContent>

</TransactionPage>
```

---

## 5. UI Components & Standardization

When building custom pages (not using the CRUD wrappers), strictly follow these guidelines to maintain consistency.

### Buttons & Icons
Only use **Syncfusion Buttons** or standard Bootstrap classes tailored for the theme.
-   **Save**: Primary Button (`e-primary`).
-   **Cancel/Back**: Flat or Outline Button.
-   **Icons**: Use Bootstrap Icons (`bi-Check`, `bi-trash`, etc.).

### Generic Dialogs
Do not create custom modal logic every time. Use the shared dialogs:

1.  **Confirmation Dialog**:
    ```csharp
    private ConfirmationDialog confirm;
    // ...
    var result = await confirm.Show("Are you sure?");
    ```

2.  **Export Dialog**:
    The `ExportDialog` component allows users to select specific columns and ranges. It is built into `MasterPage` and `TransactionPage` automatically.

---

## 6. Deployment & Running

### Running Locally
1.  Open the solution in Visual Studio.
2.  Set `TransportERP.Server` as the Startup Project.
3.  Press `F5` or `Ctrl+F5`.
4.  Alternatively, use `dotnet watch run` in the terminal for hot-reload support.

### Database Updates
1.  SQL Scripts are located in `d:\PMP_ERP\ERP\Database`.
2.  Always check for the latest script number (e.g., `11_Complete_Menu_Icons_Update.sql`) before running the app to ensure your local DB schema is up to date.

