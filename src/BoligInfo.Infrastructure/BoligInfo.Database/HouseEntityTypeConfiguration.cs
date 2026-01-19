using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoligInfo.Database;

/// <summary>
/// Entity Framework configuration for the <see cref="House"/> entity.
/// Defines table mapping, property constraints, relationships,
/// and database-level validation rules.
/// </summary>
public class HouseEntityTypeConfiguration : IEntityTypeConfiguration<House>
{
    /// <summary>
    /// Configures the database schema for the <see cref="House"/> entity.
    /// </summary>
    /// <param name="builder">
    /// The builder used to configure the <see cref="House"/> entity.
    /// </param>
    public void Configure(EntityTypeBuilder<House> builder)
    {
        // -------------------- TABLE CONFIGURATION -------------------- //
        
        builder.ToTable("House");
        builder.HasKey(h => h.Id);
        
        // -------------------- PRIMARY KEY -------------------- //
        
        builder
            .Property(h => h.Id)
            .HasColumnName("ID")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();
        
        // -------------------- PROPERTIES -------------------- //
        
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
        
        // -------------------- RELATIONSHIPS -------------------- //
        
        builder
            .HasOne<Equity>()
            .WithMany(e => e.Houses)
            .HasForeignKey(h => h.EquityId)
            .HasConstraintName("House_EquityID_fkey")
            .OnDelete(DeleteBehavior.Cascade);
        
        // -------------------- CHECK CONSTRAINTS -------------------- //
        
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