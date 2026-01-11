using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;


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
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{Cash}"/> representing all Cash entities in the database.
    /// </summary>
    public DbSet<Cash> AllCash { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // It maps the C# Enum to the Postgres Type.
        modelBuilder.HasPostgresEnum<LoanType>("LoanType", null, new NpgsqlNullNameTranslator());
        
        base.OnModelCreating(modelBuilder);
        
        // Configure entities
        new EquityEntityTypeConfiguration().Configure(modelBuilder.Entity<Equity>());
        new LoanEntityTypeConfiguration().Configure(modelBuilder.Entity<Loan>());
        new CashEntityTypeConfiguration().Configure(modelBuilder.Entity<Cash>());
    }
}