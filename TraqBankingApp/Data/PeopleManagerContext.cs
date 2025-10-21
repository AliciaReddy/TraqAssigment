using Microsoft.EntityFrameworkCore;
using TraqBankingApp.Models;

namespace TraqBankingApp.Data;
public class PeopleManagerContext : DbContext
{
    public PeopleManagerContext(DbContextOptions<PeopleManagerContext> options) : base(options) {}

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<TransactionEntry> Transactions => Set<TransactionEntry>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<UserLogin> UserLogins => Set<UserLogin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Persons
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("Persons");
            entity.HasKey(e => e.Code).HasName("PK_Persons");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
            entity.Property(e => e.Surname).HasColumnName("surname").HasMaxLength(50);
            entity.Property(e => e.IdNumber).HasColumnName("id_number").HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.IdNumber).IsUnique();
        });

        // Accounts
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(e => e.Code).HasName("PK_Accounts");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.PersonCode).HasColumnName("person_code").IsRequired();
            entity.Property(e => e.AccountNumber).HasColumnName("account_number").HasMaxLength(50).IsRequired();
            entity.Property(e => e.OutstandingBalance).HasColumnName("outstanding_balance").HasColumnType("money").IsRequired();
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.HasOne(e => e.Person)
                  .WithMany(p => p.Accounts)
                  .HasForeignKey(e => e.PersonCode)
                  .HasConstraintName("FK_Account_Person")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Transactions
        modelBuilder.Entity<TransactionEntry>(entity =>
        {
            entity.ToTable("Transactions");
            entity.HasKey(e => e.Code).HasName("PK_Transactions");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.AccountCode).HasColumnName("account_code").IsRequired();
            entity.Property(e => e.TransactionDate).HasColumnName("transaction_date").HasColumnType("datetime").IsRequired();
            entity.Property(e => e.CaptureDate).HasColumnName("capture_date").HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("money").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(100).IsRequired();
            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(e => e.AccountCode)
                  .HasConstraintName("FK_Transaction_Account")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Status
        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");
            entity.HasKey(e => e.Code).HasName("PK_Status");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        });

        // UserLogins
        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.ToTable("UserLogins");
            entity.HasKey(e => e.Id).HasName("PK_UserLogins");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Status values --> Open and Closed
        modelBuilder.Entity<Status>().HasData(
            new Status { Code = 1, Name = "Open" },
            new Status { Code = 2, Name = "Closed" }
        );
    }
}
