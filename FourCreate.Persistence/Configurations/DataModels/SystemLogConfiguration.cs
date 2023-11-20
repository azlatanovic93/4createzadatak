using FourCreate.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FourCreate.Persistence.Configurations.DataModels
{
    public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
    {
        public void Configure(EntityTypeBuilder<SystemLog> builder)
        {
            builder.ToTable("SystemLog", "FourCreate");

            builder.HasKey(sl => sl.Id);

            builder.Property(sl => sl.ResourceType)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(sl => sl.Event)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(sl => sl.Changeset)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(sl => sl.Comment)
                .IsRequired()
                .HasMaxLength(48);

            builder.Property(sl => sl.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.Property(sl => sl.CreatedBy)
                .IsRequired()
                .HasMaxLength(24);
        }
    }
}
