using Microsoft.EntityFrameworkCore;
using library_management.Entities;
namespace library_management.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<BorrowedBook> BorrowedBooks => Set<BorrowedBook>();
    public DbSet<Library> Libraries => Set<Library>();
    public DbSet<Member> Members => Set<Member>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>()
            .HasIndex(m => new { m.NationalCode, m.LibraryId })
            .IsUnique();
    }
}