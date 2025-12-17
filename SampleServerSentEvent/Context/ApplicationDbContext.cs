using Microsoft.EntityFrameworkCore;
using SampleServerSentEvent.Models;

namespace SampleServerSentEvent.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
            new Category { Id = 3, Name = "History", DisplayOrder = 3 }
        );
    }
    
    public DbSet<Category> Categories { get; set; }
}