using FastCost.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastCost.DAL;

public partial class AppDbContext : DbContext
{
    public const string DatabaseFilename = "FastCostDbSQLite.db3";
    public static string DbPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);

    public DbSet<Cost> Costs { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cost>()
            .HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .IsRequired(false);

        modelBuilder.Entity<Category>().HasData(
            new() { Id = 1, Name = "food" },
            new() { Id = 2, Name = "apartment" },
            new() { Id = 3, Name = "shopping" },
            new() { Id = 4, Name = "transport" },
            new() { Id = 5, Name = "trip" },
            new() { Id = 6, Name = "bank" },
            new() { Id = 7, Name = "company" },
            new() { Id = 8, Name = "other" }
        );
    }
}