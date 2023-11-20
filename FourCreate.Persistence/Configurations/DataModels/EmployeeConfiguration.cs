using FourCreate.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FourCreate.Persistence.Configurations.DataModels
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee", "FourCreate");

            builder.HasKey(e => e.Id);


            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasAlternateKey(e => e.Email)
                .HasName("UQ_Employee_Email");

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(24);
        }
    }
}
