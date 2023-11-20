using FourCreate.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FourCreate.Persistence.Configurations.DataModels
{
    public class EmploymentConfiguration : IEntityTypeConfiguration<Employment>
    {
        public void Configure(EntityTypeBuilder<Employment> builder)
        {
            builder.ToTable("Employment", "FourCreate");

            // composite primary key:
            builder.HasKey(employment => new
            {
                employment.CompanyId,
                employment.EmployeeId
            });

            // every employment config has a mandatory foreign key to Employee in EmployeeId
            // which represent a one-to-many (one Employee has many Employments)
            builder.HasOne(employment => employment.Employee)
                .WithMany(employee => employee.Employments)
                .HasForeignKey(employment => employment.EmployeeId);

            // every employment config has a mandatory foreign key to Company in CompanyId
            // which represent a one-to-many (one Company has many Employments)
            builder.HasOne(employment => employment.Company)
                .WithMany(company => company.Employments)
                .HasForeignKey(employment => employment.CompanyId);

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(24);
        }
    }
}
