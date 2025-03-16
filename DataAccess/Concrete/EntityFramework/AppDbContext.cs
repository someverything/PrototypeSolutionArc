using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

namespace DataAccess.Concrete.EntityFramework;

public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=localhost;Database=SolutionArchDb;User=root;Password=SomePassword_123;";
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        optionsBuilder.UseMySql(connectionString, serverVersion);
    }

    public DbSet<Test> Tests { get; set; }
    public DbSet<TestLang> TestLangs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasMany(t => t.TestLangs)
                .WithOne(tl => tl.Test)
                .HasForeignKey(tl => tl.TestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TestLang>(entity =>
        {
            entity.HasKey(tl => tl.Id);
            entity.Property(tl => tl.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(tl => tl.LangCode)
                .IsRequired()
                .HasMaxLength(10);
        });

    }
}