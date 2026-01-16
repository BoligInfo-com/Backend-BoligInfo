using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;


namespace BoligInfo.Database;


public class BoligInfoDbContext(DbContextOptions<BoligInfoDbContext> options) : DbContext(options)
{
    public DbSet<Loan> Loans { get; set; }
    
    public DbSet<Equity> Equities { get; set; }
    
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