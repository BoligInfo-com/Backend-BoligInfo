using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace BoligInfo.Database;

/// <summary>
/// Represents the database context for the BoligInfo application.
/// </summary>
public class BoligInfoDbContext(DbContextOptions<BoligInfoDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the <see cref="DbSet{Loan}"/> representing all loans in the database.
    /// </summary>
    public DbSet<Loan> Loans { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{Equity}"/> representing all equities in the database.
    /// </summary>
    public DbSet<Equity> Equities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Equity entity
        modelBuilder.Entity<Equity>(entity =>
        {
            // Set table name
            entity.ToTable("Equity");
            
            // Configure primary key
            entity.HasKey(e => e.Id);
            
            // Map property names to database column names
            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .HasColumnType("bigint")
                .ValueGeneratedOnAdd(); // Corresponds to nextval() in DB
            
            entity.Property(e => e.Currency)
                .HasColumnName("Currency")
                .HasColumnType("character varying(10)")
                .HasMaxLength(10)
                .HasDefaultValue("DKK")
                .IsRequired(false); // Not NULL is disabled
        });

        // Configure Loan entity
        modelBuilder.Entity<Loan>(entity =>
        {
            // Set table name
            entity.ToTable("Loan");
            
            // Configure primary key
            entity.HasKey(l => l.Id);
            
            // Map property names to database column names
            entity.Property(l => l.Id)
                .HasColumnName("ID")
                .HasColumnType("bigint")
                .ValueGeneratedOnAdd(); // Corresponds to nextval() in DB
            
            entity.Property(l => l.LoanType)
                .HasColumnName("LoanType")
                .HasColumnType("\"LoanType\"") // Custom type in quotes
                .HasConversion<string>() // Convert enum to string
                .IsRequired(false); // Not NULL is disabled in your schema
            
            entity.Property(l => l.LoanAmount)
                .HasColumnName("LoanAmount")
                .HasColumnType("double precision")
                .HasDefaultValue(0)
                .IsRequired(); // Not NULL is enabled
            
            entity.Property(l => l.InterestRate)
                .HasColumnName("Rate")
                .HasColumnType("double precision")
                .IsRequired(); // Not NULL is enabled
            
            entity.Property(l => l.LoanLifetime)
                .HasColumnName("LoanLifetime")
                .HasColumnType("integer")
                .IsRequired(false); // Not NULL is disabled
            
            entity.Property(l => l.EquityId)
                .HasColumnName("EquityID")
                .HasColumnType("bigint")
                .IsRequired(); // Not NULL is enabled
            
            // Configure foreign key relationship
            entity.HasOne<Equity>()
                .WithMany(e => e.Loans)
                .HasForeignKey(l => l.EquityId)
                .HasConstraintName("Loan_EquityID_fkey")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}