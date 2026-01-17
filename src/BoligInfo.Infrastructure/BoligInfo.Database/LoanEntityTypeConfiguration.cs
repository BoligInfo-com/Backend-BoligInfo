using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoligInfo.Database;

/// <summary>
/// Entity type configuration for the <see cref="Loan"/> entity.
/// Sets table name, keys, properties, foreign key relationships, and constraints.
/// </summary>
public class LoanEntityTypeConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loan");
        builder.HasKey(l => l.Id);
        
        builder
            .Property(l => l.Id)
            .HasColumnName("ID")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(l => l.LoanType)
            .HasColumnName("LoanType")
            .IsRequired(false);
        
        builder
            .Property(l => l.LoanAmount)
            .HasColumnName("LoanAmount")
            .HasColumnType("double precision")
            .HasDefaultValue(0)
            .IsRequired();
        
        builder
            .Property(l => l.InterestRate)
            .HasColumnName("Rate")
            .HasColumnType("double precision")
            .IsRequired();
        
        builder
            .Property(l => l.LoanLifetime)
            .HasColumnName("LoanLifetime")
            .HasColumnName("LoanLifetime")
            .IsRequired(false);
       
        builder
            .Property(l => l.EquityId)
            .HasColumnName("EquityID")
            .HasColumnType("bigint")
            .IsRequired();
        
        // Configure foreign key relationship
        builder
            .HasOne<Equity>()
            .WithMany(e => e.Loans)
            .HasForeignKey(l => l.EquityId)
            .HasConstraintName("Loan_EquityID_fkey")
            .OnDelete(DeleteBehavior.Cascade);
        
        // Add check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Loan_LoanAmount_NonNegative",
                "\"LoanAmount\" >= 0"
                );
            t.HasCheckConstraint(
                "CK_Loan_LoanLifetime_Positive", 
                "\"LoanLifetime\" IS NULL OR \"LoanLifetime\" > 0"
                );
        });
    }
}