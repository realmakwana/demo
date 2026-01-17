using Microsoft.EntityFrameworkCore;
using ERP.Models.Entities;
using ERP.Models.DbContext;

namespace ERP.Models.Services;

public interface IStudentService
{
    Task<List<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentByIdAsync(int studentId);
    Task<Student> CreateStudentAsync(Student student);
    Task<Student> UpdateStudentAsync(Student student);
    Task<bool> DeleteStudentAsync(int studentId);
    Task<int> GetTotalStudentsCountAsync();
    Task<List<Student>> GetActiveStudentsAsync();
}

public class StudentService : IStudentService
{
    private readonly IDbContextFactory<ERPDbContext> _contextFactory;

    public StudentService(IDbContextFactory<ERPDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Students
            .OrderByDescending(s => s.StudentID)
            .ToListAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(int studentId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Students
            .FirstOrDefaultAsync(s => s.StudentID == studentId);
    }

    public async Task<Student> CreateStudentAsync(Student student)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        student.CreatedDate = DateTime.Now;
        context.Students.Add(student);
        await context.SaveChangesAsync();

        return student;
    }

    public async Task<Student> UpdateStudentAsync(Student student)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        student.ModifiedDate = DateTime.Now;
        context.Students.Update(student);
        await context.SaveChangesAsync();

        return student;
    }

    public async Task<bool> DeleteStudentAsync(int studentId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var student = await context.Students.FindAsync(studentId);
        if (student == null)
            return false;

        context.Students.Remove(student);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetTotalStudentsCountAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Students.CountAsync();
    }

    public async Task<List<Student>> GetActiveStudentsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Students
            .Where(s => s.IsActive == true)
            .OrderByDescending(s => s.StudentID)
            .ToListAsync();
    }
}
