using FourCreate.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FourCreate.Persistence.Configurations.DataModels
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company", "FourCreate");

            builder.HasKey(c => c.Id);

            builder.HasAlternateKey(c => c.Name)
                .HasName("UQ_Company_Name");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.Property(c => c.CreatedBy)
                .IsRequired()
                .HasMaxLength(24);
        }
    }
}
