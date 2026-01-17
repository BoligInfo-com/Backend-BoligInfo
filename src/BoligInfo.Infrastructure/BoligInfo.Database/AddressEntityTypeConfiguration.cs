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
            .HasMaxLength(180)
            .IsRequired();
    }
}