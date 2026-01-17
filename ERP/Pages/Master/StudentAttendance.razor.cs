using Microsoft.AspNetCore.Components;
using ERP.Models.Services;
using ERP.Components.Shared.UI;
using StudentAttendanceEntity = ERP.Models.Entities.StudentAttendance;
using ERP.Models.Entities;
using Syncfusion.Blazor.DropDowns;

namespace ERP.Pages.Master
{
    public partial class StudentAttendance : ComponentBase
    {
        [Inject] private IStudentAttendanceService StudentAttendanceService { get; set; } = default!;
        [Inject] private IStudentService StudentService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IMenuService MenuService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<AppBreadcrumbItem> breadcrumbItems = new()
        {
            new() { Label = "Home", Url = "/" },
            new() { Label = "Master", Url = "#" },
            new() { Label = "Student Attendance", Url = "/StudentAttendance" }
        };

        // ---------------- Data ----------------
        private List<StudentAttendanceEntity> attendanceList = new();
        private List<Student> students = new();

        private StudentAttendanceEntity? currentAttendance;
        private bool showForm = false;
        private UserWiseMenu? currentRights;

        // Multiple Attendance
        private bool showMultipleAttendanceForm = false;
        private DateTime? selectedDate = DateTime.Today;
        private List<MultipleAttendanceItem> multipleAttendanceList = new();

        // ---------------- Lifecycle ----------------
        protected override async Task OnInitializedAsync()
        {
            await LoadAttendance();
            await LoadStudents();
            await LoadRights();
        }

        // ---------------- Rights ----------------
        private async Task LoadRights()
        {
            if (AuthService.IsAuthenticated)
            {
                var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
                var path = "/" + relativePath.Split('?')[0];
                currentRights = await MenuService.GetUserMenuRightsAsync(AuthService.UserId, path);
            }
        }

        // ---------------- Load Data ----------------
        private async Task LoadAttendance()
        {
            try
            {
                attendanceList = await StudentAttendanceService.GetAllAttendanceAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading attendance: {ex.Message}", ToastLevel.Error);
                attendanceList = new List<StudentAttendanceEntity>();
            }
        }

        private async Task LoadStudents()
        {
            try
            {
                students = await StudentService.GetActiveStudentsAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading students: {ex.Message}", ToastLevel.Error);
                students = new List<Student>();
            }
        }

        // ---------------- Form Actions ----------------
        private void OpenAddForm()
        {
            currentAttendance = new StudentAttendanceEntity
            {
                AttendanceDate = DateTime.Today,
                IsPresent = true
            };
            showForm = true;
        }

        private void OpenEditForm(StudentAttendanceEntity attendance)
        {
            currentAttendance = new StudentAttendanceEntity
            {
                AttendanceID = attendance.AttendanceID,
                StudentID = attendance.StudentID,
                AttendanceDate = attendance.AttendanceDate,
                IsPresent = attendance.IsPresent,
                StudentName = students
            .FirstOrDefault(s => s.StudentID == attendance.StudentID)
            ?.StudentName
            };
            showForm = true;
        }

        private void CloseForm()
        {
            showForm = false;
            currentAttendance = null;
        }

        // ---------------- Save ----------------
        private async Task SaveAttendance()
        {
            if (currentAttendance == null)
                return;

            var student = students.FirstOrDefault(s =>
                s.StudentName != null &&
                s.StudentName.Equals(currentAttendance.StudentName, StringComparison.OrdinalIgnoreCase));

            if (student == null)
            {
                ToastService.ShowToast("Please select a valid student from suggestion list", ToastLevel.Warning);
                return;
            }

            currentAttendance.StudentID = student.StudentID;

            try
            {
                if (currentAttendance.AttendanceID == 0)
                {
                    await StudentAttendanceService.CreateAttendanceAsync(currentAttendance);
                    ToastService.ShowToast("Attendance saved successfully!", ToastLevel.Success);
                }
                else
                {
                    await StudentAttendanceService.UpdateAttendanceAsync(currentAttendance);
                    ToastService.ShowToast("Attendance updated successfully!", ToastLevel.Success);
                }

                await LoadAttendance();
                CloseForm();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ex.Message, ToastLevel.Error);
            }
        }


        // ---------------- Delete ----------------
        private async Task DeleteAttendance(StudentAttendanceEntity attendance)
        {
            try
            {
                var success = await StudentAttendanceService.DeleteAttendanceAsync(attendance.AttendanceID);
                if (success)
                {
                    ToastService.ShowToast("Attendance deleted successfully!", ToastLevel.Success);
                    await LoadAttendance();
                }
                else
                {
                    ToastService.ShowToast("Attendance record not found!", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error deleting attendance: {ex.Message}", ToastLevel.Error);
            }
        }
        private async Task<IEnumerable<string>> SearchStudents(string searchText)
        {
            await Task.Delay(100);

            if (string.IsNullOrWhiteSpace(searchText))
                return Enumerable.Empty<string>();

            return students
                .Where(s => !string.IsNullOrEmpty(s.StudentName) &&
                            s.StudentName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.StudentName!)
                .Distinct()
                .Take(50);
        }

        // ---------------- Multiple Attendance Methods ----------------
        private async Task OpenMultipleAttendanceForm()
        {
            showMultipleAttendanceForm = true;
            selectedDate = DateTime.Today;
            await LoadMultipleAttendanceData();
        }

        private void CloseMultipleAttendanceForm()
        {
            showMultipleAttendanceForm = false;
            multipleAttendanceList.Clear();
            selectedDate = DateTime.Today;
        }

        private async Task OnDateChangedAsync()
        {
            Console.WriteLine($"Date changed to: {selectedDate?.ToString("yyyy-MM-dd") ?? "null"}");
            if (selectedDate != null)
            {
                await LoadMultipleAttendanceData();
            }
        }

        private async Task LoadMultipleAttendanceData()
        {
            if (selectedDate == null)
                return;

            try
            {
                // Clear the existing list first
                multipleAttendanceList.Clear();
                
                // Normalize the date to remove time component
                var dateOnly = selectedDate.Value.Date;
                
                Console.WriteLine($"Loading attendance for date: {dateOnly:yyyy-MM-dd}");
                
                // Get all active students
                var activeStudents = await StudentService.GetActiveStudentsAsync();
                Console.WriteLine($"Total active students: {activeStudents.Count}");

                // Get existing attendance for the selected date
                var existingAttendance = await StudentAttendanceService.GetAttendanceByDateAsync(dateOnly);
                Console.WriteLine($"Existing attendance records for {dateOnly:yyyy-MM-dd}: {existingAttendance.Count}");

                // Create the list with all students
                multipleAttendanceList = activeStudents.Select(student =>
                {
                    var existing = existingAttendance.FirstOrDefault(a => a.StudentID == student.StudentID);
                    
                    // IsPresent is checked only if there's an existing record AND it's marked as present
                    // Otherwise it's unchecked (for absent or no record)
                    bool isPresent = existing != null && existing.IsPresent;
                    
                    if (existing != null)
                    {
                        Console.WriteLine($"Student {student.StudentName} - Present: {isPresent}");
                    }
                    
                    return new MultipleAttendanceItem
                    {
                        StudentID = student.StudentID,
                        StudentName = student.StudentName ?? "",
                        AttendanceID = existing?.AttendanceID ?? 0,
                        IsPresent = isPresent,
                        HasExistingRecord = existing != null
                    };
                }).ToList();

                Console.WriteLine($"Total items in multipleAttendanceList: {multipleAttendanceList.Count}");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error loading attendance data: {ex.Message}", ToastLevel.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
        }



        private async Task SaveMultipleAttendance()
        {
            if (selectedDate == null)
            {
                ToastService.ShowToast("Please select a date", ToastLevel.Warning);
                return;
            }

            try
            {
                int savedCount = 0;
                int updatedCount = 0;

                foreach (var item in multipleAttendanceList)
                {
                    // Create attendance record for each student
                    // IsPresent = true if checkbox is checked, false if unchecked
                    var attendance = new StudentAttendanceEntity
                    {
                        AttendanceID = item.AttendanceID,
                        StudentID = item.StudentID,
                        AttendanceDate = selectedDate.Value,
                        IsPresent = item.IsPresent  // Checked = Present, Unchecked = Absent
                    };

                    if (item.HasExistingRecord && item.AttendanceID > 0)
                    {
                        // Update existing record
                        await StudentAttendanceService.UpdateAttendanceAsync(attendance);
                        updatedCount++;
                    }
                    else
                    {
                        // Create new record
                        await StudentAttendanceService.CreateAttendanceAsync(attendance);
                        savedCount++;
                    }
                }

                if (savedCount > 0 || updatedCount > 0)
                {
                    ToastService.ShowToast(
                        $"Attendance saved successfully!",
                        ToastLevel.Success);
                    await LoadAttendance();
                    CloseMultipleAttendanceForm();
                }
                else
                {
                    ToastService.ShowToast("No attendance records to save", ToastLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Error saving attendance: {ex.Message}", ToastLevel.Error);
            }
        }

    }

    // Helper class for multiple attendance
    public class MultipleAttendanceItem
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; } = "";
        public int AttendanceID { get; set; }
        public bool IsPresent { get; set; }
        public bool HasExistingRecord { get; set; }
    }
}
