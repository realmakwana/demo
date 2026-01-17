using Microsoft.EntityFrameworkCore;
using ERP.Models.Entities;
using ERP.Models.DbContext;

namespace ERP.Models.Services;

public interface IStudentAttendanceService
{
    Task<List<StudentAttendance>> GetAllAttendanceAsync();
    Task<StudentAttendance?> GetAttendanceByIdAsync(int attendanceId);
    Task<StudentAttendance?> GetAttendanceByStudentAndDateAsync(int studentId, DateTime attendanceDate);
    Task<StudentAttendance> CreateAttendanceAsync(StudentAttendance attendance);
    Task<StudentAttendance> UpdateAttendanceAsync(StudentAttendance attendance);
    Task<bool> DeleteAttendanceAsync(int attendanceId);
    Task<List<StudentAttendance>> GetAttendanceByDateAsync(DateTime attendanceDate);
}

public class StudentAttendanceService : IStudentAttendanceService
{
    private readonly IDbContextFactory<ERPDbContext> _contextFactory;

    public StudentAttendanceService(IDbContextFactory<ERPDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<StudentAttendance>> GetAllAttendanceAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StudentAttendances
            .Include(a => a.Student)
            .OrderByDescending(a => a.AttendanceDate)
            .ToListAsync();
    }

    public async Task<StudentAttendance?> GetAttendanceByIdAsync(int attendanceId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StudentAttendances
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.AttendanceID == attendanceId);
    }

    public async Task<StudentAttendance?> GetAttendanceByStudentAndDateAsync(
        int studentId, DateTime attendanceDate)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StudentAttendances
            .FirstOrDefaultAsync(a =>
                a.StudentID == studentId &&
                a.AttendanceDate == attendanceDate.Date);
    }

    public async Task<StudentAttendance> CreateAttendanceAsync(StudentAttendance attendance)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        // Prevent duplicate attendance
        var exists = await context.StudentAttendances.AnyAsync(a =>
            a.StudentID == attendance.StudentID &&
            a.AttendanceDate == attendance.AttendanceDate.Date);

        if (exists)
            throw new Exception("Attendance already exists for the selected date.");

        context.StudentAttendances.Add(attendance);
        await context.SaveChangesAsync();

        return attendance;
    }

    public async Task<StudentAttendance> UpdateAttendanceAsync(StudentAttendance attendance)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.StudentAttendances.Update(attendance);
        await context.SaveChangesAsync();

        return attendance;
    }

    public async Task<bool> DeleteAttendanceAsync(int attendanceId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var attendance = await context.StudentAttendances.FindAsync(attendanceId);

        if (attendance == null)
            return false;

        context.StudentAttendances.Remove(attendance);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<List<StudentAttendance>> GetAttendanceByDateAsync(DateTime attendanceDate)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StudentAttendances
            .Include(a => a.Student)
            .Where(a => a.AttendanceDate == attendanceDate.Date)
            .OrderBy(a => a.Student!.StudentName)
            .ToListAsync();
    }
}