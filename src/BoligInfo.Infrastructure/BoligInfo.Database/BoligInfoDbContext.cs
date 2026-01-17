using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;


namespace BoligInfo.Database;


/// <summary>
/// Database context for the BoligInfo application.
/// Configures DbSets, PostgreSQL enum mapping, and entity configurations.
/// </summary>
public class BoligInfoDbContext(DbContextOptions<BoligInfoDbContext> options) : DbContext(options)
{
    /// <summary>
    /// DbSet for loans in the system.
    /// </summary>
    public DbSet<Loan> Loans { get; set; }
    
    /// <summary>
    /// DbSet for equities in the system.
    /// </summary>
    public DbSet<Equity> Equities { get; set; }
    
    /// <summary>
    /// DbSet for cash records in the system.
    /// </summary>
    public DbSet<Cash> AllCash { get; set; }
    
    /// <summary>
    /// Configures entity mappings, PostgreSQL enum types, and constraints.
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder instance for configuring EF Core models.</param>
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