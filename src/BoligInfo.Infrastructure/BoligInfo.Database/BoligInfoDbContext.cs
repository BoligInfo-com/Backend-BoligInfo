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
    public DbSet<House> Houses { get; set; }
    public DbSet<CashFlow> CashFlows { get; set; }
    
    
    /// <summary>
    /// Configures entity mappings, PostgreSQL enum types, and constraints.
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder instance for configuring EF Core models.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map enums to Postgres types
        modelBuilder.HasPostgresEnum<LoanType>("LoanType", null, new NpgsqlNullNameTranslator());
        modelBuilder.HasPostgresEnum<EnergyLabel>("EnergyLabel", null, new NpgsqlNullNameTranslator());
        modelBuilder.HasPostgresEnum<Frequency>("Frequency", null, new NpgsqlNullNameTranslator());
        modelBuilder.HasPostgresEnum<CashFlowType>("CashFlowType", null, new NpgsqlNullNameTranslator());

        base.OnModelCreating(modelBuilder);
        
        // Configure entities
        new EquityEntityTypeConfiguration().Configure(modelBuilder.Entity<Equity>());
        new LoanEntityTypeConfiguration().Configure(modelBuilder.Entity<Loan>());
        new CashEntityTypeConfiguration().Configure(modelBuilder.Entity<Cash>());
        new HouseEntityTypeConfiguration().Configure(modelBuilder.Entity<House>());
        new CashFlowEntityTypeConfiguration().Configure(modelBuilder.Entity<CashFlow>());
    }
}