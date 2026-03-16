using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;


public sealed class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.ToTable("ShortUrls")
            .HasCharSet("utf8mb4")
            .UseCollation("utf8mb4_general_ci");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnType("binary(16)")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("char(10)");

        builder.Property(x => x.LongUrl)
            .IsRequired()
            .HasMaxLength(2048)
            .HasColumnType("varchar(2048)");
        
        builder.Property(x => x.LongUrlHash)
            .IsRequired()
            .HasColumnType("binary(32)"); 

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(x => x.Clicks)
            .IsRequired()
            .HasColumnType("int")
            .HasDefaultValue(0);

        builder.HasIndex(x => x.Code).IsUnique();
        
        builder.HasIndex(x => x.LongUrlHash).IsUnique(); 

        builder.HasIndex(x => x.CreatedAt);
    }
}