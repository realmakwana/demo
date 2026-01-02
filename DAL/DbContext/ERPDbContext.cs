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
    }
}
