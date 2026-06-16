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
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>()
            .HasIndex(m => new { m.NationalCode, m.LibraryId })
            .IsUnique();

        modelBuilder.Entity<Member>()
            .HasMany(m => m.FavoriteBooks)
            .WithMany()
            .UsingEntity(j => j.ToTable("FavoriteBooks"));
    }
}