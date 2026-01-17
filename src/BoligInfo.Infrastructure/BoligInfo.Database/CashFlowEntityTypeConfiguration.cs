using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoligInfo.Database;

public class CashFlowEntityTypeConfiguration : IEntityTypeConfiguration<CashFlow>
{
    public void Configure(EntityTypeBuilder<CashFlow> builder)
    {
        builder.ToTable("CashFlow");
        builder.HasKey(cf => cf.Id);
        
        builder
            .Property(cf => cf.Id)
            .HasColumnName("ID")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(cf => cf.Type)
            .HasColumnName("Type")
            .IsRequired();
        
        builder
            .Property(cf => cf.Frequency)
            .HasColumnName("Frequency")
            .IsRequired();
        
        builder
            .Property(cf => cf.Amount)
            .HasColumnName("Amount")
            .HasColumnType("double precision")
            .IsRequired();
        
        builder
            .Property(cf => cf.Name)
            .HasColumnName("Name")
            .HasColumnType("character varying(60)")
            .HasMaxLength(60)
            .IsRequired();
        
        builder
            .Property(cf => cf.Description)
            .HasColumnName("Description")
            .HasColumnType("character varying(1400)")
            .HasMaxLength(1400)
            .IsRequired(false);
        
        builder
            .Property(cf => cf.HouseId)
            .HasColumnName("HouseID")
            .HasColumnType("bigint")
            .IsRequired();
        
        // Configure foreign key relationship
        builder
            .HasOne<House>()
            .WithMany(h => h.CashFlows)
            .HasForeignKey(cf => cf.HouseId)
            .HasConstraintName("CashFlow_HouseID_fkey")
            .OnDelete(DeleteBehavior.Cascade);
        
        // Add check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_CashFlow_Amount_NonNegative",
                "\"Amount\" >= 0"
            );
        });
    }
}