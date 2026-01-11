using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoligInfo.Database;

public class EquityEntityTypeConfiguration : IEntityTypeConfiguration<Equity> 
{
    public void Configure(EntityTypeBuilder<Equity> builder)
    {
        builder.ToTable("Equity");
        builder.HasKey(e => e.Id);
        builder
            .Property(e => e.Id)
            .HasColumnName("ID")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();
        builder
            .Property(e => e.Currency)
            .HasColumnName("Currency")
            .HasColumnType("character varying(10)")
            .HasMaxLength(10)
            .HasDefaultValue("DKK")
            .IsRequired(false);
        
    }
}