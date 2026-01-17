using Microsoft.EntityFrameworkCore;
using ERP.Models.Entities;


namespace ERP.Models.DbContext;

public class ERPDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ERPDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<ERPDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<UserCategory> UserCategories { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<UserWiseMenu> UserWiseMenus { get; set; }
    public DbSet<UserWiseRights> UserWiseRights { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<StudentAttendance> StudentAttendances { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Category> Category { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Company entity configuration
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyID);
            
            entity.Property(e => e.Mail)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.MailKey)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.CompanyName)
                  .HasMaxLength(100);

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
        });

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID);
            
            entity.HasIndex(e => e.EmailID);
            
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.UserName)
                  .HasMaxLength(150);

            entity.Property(e => e.Password)
                  .HasMaxLength(150);

            entity.Property(e => e.EmailID)
                  .HasMaxLength(500);

            entity.Property(e => e.MobileNo)
                  .HasMaxLength(10);

            entity.Property(e => e.MAC)
                  .HasMaxLength(1000);

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);

            // Configure relationship with Company
            entity.HasOne(e => e.Company)
                  .WithMany(c => c.Users)
                  .HasForeignKey(e => e.CompanyID)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Student entity configuration
        //modelBuilder.Entity<Student>(entity =>
        //{
        //    entity.HasKey(e => e.StudentID);
            
        //    entity.HasIndex(e => e.StudentName);
            
        //    entity.HasIndex(e => e.IsActive);

        //    entity.Property(e => e.StudentName)
        //          .IsRequired()
        //          .HasMaxLength(100);

        //    entity.Property(e => e.PhoneNumber)
        //          .HasMaxLength(15);

        //    entity.Property(e => e.IsActive)
        //          .HasDefaultValue(true);
        //});

        // Customer entity configuration
        //modelBuilder.Entity<Customer>(entity =>
        //{
        //    entity.HasKey(e => e.);
            
        //    entity.HasIndex(e => e.CustomerName);
            
        //    entity.HasIndex(e => e.Email);
            
        //    entity.HasIndex(e => e.IsActive);

        //    entity.Property(e => e.CustomerName)
        //          .IsRequired()
        //          .HasMaxLength(50);

        //    entity.Property(e => e.Email)
        //          .IsRequired()
        //          .HasMaxLength(50);

        //    entity.Property(e => e.PhoneNumber)
        //          .IsRequired()
        //          .HasMaxLength(50);

        //    entity.Property(e => e.IsActive)
        //          .HasDefaultValue(true);
        //});

        // Invoice entity configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceID);
            
            entity.HasIndex(e => e.invoice_no).IsUnique();
            entity.HasIndex(e => e.invoice_date);
            entity.HasIndex(e => e.CustomerID);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.invoice_no)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.invoice_date)
                  .IsRequired();

            entity.Property(e => e.CustomerID)
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);

            // Configure relationship with InvoiceItems
            entity.HasMany(e => e.Items)
                  .WithOne()
                  .HasForeignKey(i => i.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // InvoiceItem entity configuration
        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.LineItemID);
            
            entity.HasIndex(e => e.InvoiceId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.item_name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.remarks)
                  .HasMaxLength(500);

            entity.Property(e => e.qty)
                  .IsRequired();

            entity.Property(e => e.rate)
                  .HasPrecision(18, 2)
                  .IsRequired();

            entity.Property(e => e.discount)
                  .HasPrecision(18, 2);

            entity.Property(e => e.amount)
                  .HasPrecision(18, 2);

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
        });
    }
}
