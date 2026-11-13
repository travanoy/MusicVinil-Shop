namespace Proekt.Data;

using Microsoft.EntityFrameworkCore;
using Proekt.Models;
using Proekt.Enums;
using Microsoft.Extensions.Logging;

public class MusicStoreDbContext : DbContext
{
    public MusicStoreDbContext() { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=Poekt;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False")
            .LogTo(Console.WriteLine, LogLevel.Information);
    }


    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Record> Records => Set<Record>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<PromotionCondition> PromotionConditions => Set<PromotionCondition>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Employee>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Login).IsRequired().HasMaxLength(100);
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
            e.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(50);
            e.HasIndex(x => x.Login).IsUnique();
        });

        mb.Entity<Customer>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Login).IsRequired().HasMaxLength(100);
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
            e.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            e.Property(x => x.TotalSpent).HasPrecision(18, 2);
            e.HasIndex(x => x.Login).IsUnique();
            
        });

        mb.Entity<Record>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Artist).IsRequired().HasMaxLength(200);
            e.Property(x => x.Publisher).IsRequired().HasMaxLength(200);
            e.Property(x => x.CostPrice).HasPrecision(18, 2);
            e.Property(x => x.SellingPrice).HasPrecision(18, 2);
            e.Property(x => x.Genre).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            e.HasIndex(x => x.Title);
            e.HasIndex(x => x.Artist);
            e.HasIndex(x => x.Genre);
            e.HasIndex(x => x.Status);

      
        });

        mb.Entity<Sale>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
            e.Property(x => x.DiscountPercent).HasPrecision(5, 2);
            e.Property(x => x.DiscountedAmount).HasPrecision(18, 2);
            e.HasIndex(x => x.SaleDate);

            e.HasOne(x => x.Employee)
             .WithMany(emp => emp.Sales)
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Customer)
             .WithMany(c => c.Purchases)
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<SaleItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.Property(x => x.PromotionDiscount).HasPrecision(5, 2);
            e.Property(x => x.FinalPrice).HasPrecision(18, 2);
            e.Property(x => x.AppliedPromotionName).HasMaxLength(200);

            e.HasOne(x => x.Sale)
             .WithMany(s => s.Items)
             .HasForeignKey(x => x.SaleId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Record)
             .WithMany(r => r.SaleItems)
             .HasForeignKey(x => x.RecordId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Promotion>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.DiscountPercent).HasPrecision(5, 2);
            e.HasIndex(x => x.StartDate);
            e.HasIndex(x => x.EndDate);
            e.Ignore(x => x.IsActive);
        });

        mb.Entity<PromotionCondition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Field).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.Operator).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.Value).IsRequired().HasMaxLength(200);
            e.Property(x => x.ValueMax).HasMaxLength(200);

            e.HasOne(x => x.Promotion)
             .WithMany(p => p.Conditions)
             .HasForeignKey(x => x.PromotionId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}