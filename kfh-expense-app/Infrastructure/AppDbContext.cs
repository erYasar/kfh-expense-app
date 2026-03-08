using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using kfh_expense_app.Domain.Entities;

namespace kfh_expense_app.Infrastructure;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Currency).HasMaxLength(3);
        });
    }
}
