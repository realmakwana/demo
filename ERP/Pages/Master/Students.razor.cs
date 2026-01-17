using Microsoft.AspNetCore.Components;
using ERP.Models.Services;
using ERP.Components.Shared.UI;
using StudentEntity = ERP.Models.Entities.Student;
using ERP.Models.Entities;

namespace ERP.Pages.Master
{
    public partial class Students : ComponentBase
    {
        [Inject] private IStudentService StudentService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Master", Url = "#" },
            new() { Label = "Students", Url = "/students" }
        };

        private List<StudentEntity> students = new();
        private StudentEntity? currentStudent;
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        protected override async Task OnInitializedAsync()
        {
            await LoadStudents();
            await LoadRights();
        }

        private async Task LoadRights()
        {
            if (AuthService.IsAuthenticated)
            {
                var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
                var path = "/" + relativePath.Split('?')[0];
                currentRights = await MenuService.GetUserMenuRightsAsync(AuthService.UserId, path);
            }
        }

        private async Task LoadStudents()
        {
            try
            {
                students = await StudentService.GetAllStudentsAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading students: {ex.Message}", ToastLevel.Error);
                students = new List<StudentEntity>();
            }
        }

        private void OpenAddForm()
        {
            currentStudent = new StudentEntity
            {
                IsActive = true
            };
            showForm = true;
        }

        private void OpenEditForm(StudentEntity student)
        {
            currentStudent = new StudentEntity
            {
                StudentID = student.StudentID,
                StudentName = student.StudentName,
                PhoneNumber = student.PhoneNumber,
                IsActive = student.IsActive,
                CreatedDate = student.CreatedDate,
                ModifiedDate = student.ModifiedDate
            };
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentStudent = null;
        }

        private async Task SaveStudent()
        {
            if (currentStudent == null) return;

            try
            {
                if (currentStudent.StudentID == 0)
                {
                    await StudentService.CreateStudentAsync(currentStudent);
                    ToastService.ShowToast("Student created successfully!", ToastLevel.Success);
                }
                else
                {
                    await StudentService.UpdateStudentAsync(currentStudent);
                    ToastService.ShowToast("Student updated successfully!", ToastLevel.Success);
                }

                await LoadStudents();
                CloseForm();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving student: {ex.Message}", ToastLevel.Error);
            }
        }

        private async Task DeleteStudent(StudentEntity student)
        {
            try
            {
                var success = await StudentService.DeleteStudentAsync(student.StudentID);
                if (success)
                {
                    ToastService.ShowToast("Student deleted successfully!", ToastLevel.Success);
                    await LoadStudents();
                }
                else
                {
                    ToastService.ShowToast("Student not found!", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error deleting student: {ex.Message}", ToastLevel.Error);
            }
        }
    }
}
