using Microsoft.AspNetCore.Components;
using TransportERP.Models.Entities;
using TransportERP.Models.DTOs;
using TransportERP.Models.Services;
using Syncfusion.Blazor.Grids;
using ERP.Components.Shared.UI;

namespace ERP.Pages
{
    public partial class UserRights : ComponentBase
    {
        [Inject] private IUserService UserService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<UserDto> users = new();
        private int selectedUserId;
        private List<MenuRightsDto> menuRights = new();
        private bool isLoading = false;
        private SfGrid<MenuRightsDto>? grid;
        
        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Settings", Url = "/settings" },
            new() { Label = "User Menu Rights", Url = "/user-rights" }
        };

        protected override async Task OnInitializedAsync()
        {
            users = await UserService.GetAllUsersAsync();
        }

        private async Task OnUserSelectedAsync(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, UserDto> args)
        {
            if (args.Value > 0)
            {
                selectedUserId = args.Value;
                await LoadUserRights();
            }
            else
            {
                selectedUserId = 0;
                menuRights = new();
            }
        }

        private async Task LoadUserRights()
        {
            if (selectedUserId == 0) return;

            isLoading = true;
            var allMenus = await MenuService.GetAllMenusAsync();
            var userRights = await MenuService.GetUserRightsListAsync(selectedUserId);

            menuRights = allMenus.Select(m => {
                var existing = userRights.FirstOrDefault(ur => ur.MenuID == m.MenuID);
                return new MenuRightsDto
                {
                    MenuID = m.MenuID,
                    MenuDispName = m.MenuDispName,
                    ParentMenuID = m.ParentMenuID,
                    IsShow = existing?.IsShow ?? false,
                    IsAdd = existing?.IsAdd ?? false,
                    IsEdit = existing?.IsEdit ?? false,
                    IsDelete = existing?.IsDelete ?? false,
                    IsPrint = existing?.IsPrint ?? false,
                    IsExport = existing?.IsExport ?? false,
                    IsVerify = existing?.IsVerify ?? false,
                    UserWiseMenuID = existing?.UserWiseMenuID ?? 0
                };
            }).ToList();

            isLoading = false;
            StateHasChanged();
        }

        private void SetAllRights(bool status)
        {
            foreach (var item in menuRights)
            {
                item.IsShow = status;
                item.IsAdd = status;
                item.IsEdit = status;
                item.IsDelete = status;
                item.IsPrint = status;
                item.IsExport = status;
                item.IsVerify = status;
            }
            StateHasChanged();
        }

        private async Task SaveRights()
        {
            if (selectedUserId == 0) return;

            isLoading = true;
            var rightsToUpdate = menuRights.Select(mr => new UserWiseMenu
            {
                UserWiseMenuID = mr.UserWiseMenuID,
                UserID = selectedUserId,
                MenuID = mr.MenuID,
                IsShow = mr.IsShow,
                IsAdd = mr.IsAdd,
                IsEdit = mr.IsEdit,
                IsDelete = mr.IsDelete,
                IsPrint = mr.IsPrint,
                IsExport = mr.IsExport,
                IsVerify = mr.IsVerify,
                IsActive = true
            }).ToList();

            var success = await MenuService.UpdateUserMenuRightsAsync(rightsToUpdate);
            if (success)
            {
                ToastService.ShowToast("Rights updated successfully!", ToastLevel.Success);
                await LoadUserRights(); // Refresh to get new IDs
            }
            else
            {
                ToastService.ShowToast("Failed to update rights.", ToastLevel.Error);
            }
            isLoading = false;
        }

    }
}
