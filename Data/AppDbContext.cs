using Microsoft.EntityFrameworkCore;
using library_management.Entities;
namespace library_management.Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
}