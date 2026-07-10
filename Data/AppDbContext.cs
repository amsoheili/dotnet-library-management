using Microsoft.EntityFrameworkCore;
using library_management.Entities;
namespace library_management.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<LibraryUser> LibraryUsers => Set<LibraryUser>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<BorrowedBook> BorrowedBooks => Set<BorrowedBook>();
    public DbSet<Library> Libraries => Set<Library>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PersonRole> PersonRoles => Set<PersonRole>();
    public DbSet<Wallet> Wallets => Set<Wallet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryUser>()
            .HasIndex(m => new { m.NationalCode, m.LibraryId })
            .IsUnique();

        modelBuilder.Entity<LibraryUser>()
            .HasMany(m => m.FavoriteBooks)
            .WithMany()
            .UsingEntity(j => j.ToTable("FavoriteBooks"));

        modelBuilder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasPrecision(18, 4);
    }
}