using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoligInfo.Database;

public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Address");
        builder.HasKey(a => a.Id);
        
        builder
            .Property(a => a.Id)
            .HasColumnName("ID")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(a => a.Country)
            .HasColumnName("Country")
            .HasColumnType("character varying(180)")
            .HasDefaultValue("Denmark")
            .HasMaxLength(180)
            .IsRequired();
        
        builder
            .Property(a => a.City)
            .HasColumnName("City")
            .HasColumnType("character varying(340)")
            .HasMaxLength(340)
            .IsRequired();
        
        builder
            .Property(a => a.Zipcode)
            .HasColumnName("Zipcode")
            .HasColumnType("character varying(80)")
            .HasMaxLength(80)
            .IsRequired();
        
        builder
            .Property(a => a.Street)
            .HasColumnName("Street")
            .HasColumnType("character varying(34o)")
            .HasMaxLength(340)
            .IsRequired();
        
        builder
            .Property(a => a.Number)
            .HasColumnName("Number")
            .HasColumnType("character varying(20)")
            .HasMaxLength(20)
            .IsRequired(false);
        
        builder
            .Property(a => a.Suite)
            .HasColumnName("Suite")
            .HasColumnType("character varying(20)")
            .HasMaxLength(20)
            .IsRequired(false);
        
        builder
            .Property(a => a.Floor)
            .HasColumnName("Floor")
            .HasColumnType("integer")
            .IsRequired(false);
        
        builder
            .Property(a => a.HouseId)
            .HasColumnName("HouseId")
            .HasColumnType("bigint")
            .IsRequired();
        
        builder
            .HasOne<House>()
            .WithOne(h => h.Address)
            .HasForeignKey<Address>(a => a.HouseId)
            .IsRequired();
    }
}