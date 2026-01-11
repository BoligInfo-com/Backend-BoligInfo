using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoligInfo.Database;

public class CashEntityTypeConfiguration : IEntityTypeConfiguration<Cash>
{
    public void Configure(EntityTypeBuilder<Cash> builder)
    {
        builder.ToTable("Cash");
        builder.HasKey(c => c.Id);
        builder
            .Property(c => c.Id)
            .HasColumnName("ID")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();
        builder
            .Property(c => c.CashAmount)
            .HasColumnName("CashAmount")
            .HasColumnType("double precision")
            .HasDefaultValue(0)
            .IsRequired();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Cash_CashAmount_NonNegative",
                "\"CashAmount\" >= 0"
            );
        });
    }
}