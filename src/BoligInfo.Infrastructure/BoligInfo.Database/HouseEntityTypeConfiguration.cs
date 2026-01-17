using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoligInfo.Database;

public class HouseEntityTypeConfiguration : IEntityTypeConfiguration<House>
{
    public void Configure(EntityTypeBuilder<House> builder)
    {
        builder.ToTable("House");
        builder.HasKey(h => h.Id);
        
        builder
            .Property(h => h.Id)
            .HasColumnName("ID")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(h => h.Price)
            .HasColumnName("Price")
            .HasColumnType("double precision")
            .IsRequired();
        
        builder
            .Property(h => h.NumberOfRooms)
            .HasColumnName("NumberOfRooms")
            .HasColumnType("integer")
            .IsRequired(false);
        
        builder
            .Property(h => h.SquareMeters)
            .HasColumnName("SquareMeters")
            .HasColumnType("integer")
            .IsRequired(false);
        
        builder
            .Property(h => h.EnergyLabel)
            .HasColumnName("EnergyLabel")
            .IsRequired(false);
        
        builder
            .Property(h => h.PurchaseDate)
            .HasColumnName("PurchaseDate")
            .HasColumnType("date")
            .IsRequired(false);
        
        builder
            .Property(h => h.EquityId)
            .HasColumnName("EquityID")
            .HasColumnType("bigint")
            .IsRequired();
        
        // Configure foreign key relationship
        builder
            .HasOne<Equity>()
            .WithMany(e => e.Houses)
            .HasForeignKey(h => h.EquityId)
            .HasConstraintName("House_EquityID_fkey")
            .OnDelete(DeleteBehavior.Cascade);
        
        // Add check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_House_Price_NonNegative",
                "\"Price\" >= 0"
            );
            t.HasCheckConstraint(
                "CK_House_NumberOfRooms_NonNegative",
                "\"NumberOfRooms\" IS NULL OR \"NumberOfRooms\" >= 0"
            );
            t.HasCheckConstraint(
                "CK_House_SquareMeters_NonNegative",
                "\"SquareMeters\" IS NULL OR \"SquareMeters\" >= 0"
            );
        });
    }
}