using CSharpFunctionalExtensions;
using FourCreate.Persistence.DataModels;
using FourCreate.Persistence.DataModels.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FourCreate.Persistence
{
    public class NpgsqlDbContext : DbContext
    {
        public NpgsqlDbContext(DbContextOptions<NpgsqlDbContext> options)
            : base(options)
        {
        }

        public virtual async Task<int> SaveChangesAsync(string username = "SYSTEM")
        {
            foreach (var entry in base.ChangeTracker.Entries<BaseDataModel>()
                .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified).ToList())
            {
                DateTime dateTimeNow = DateTime.UtcNow;

                SystemLog systemLogEntry = new SystemLog()
                {
                    Changeset = "",
                    CreatedAt = dateTimeNow,
                    CreatedBy = username
                };

                if (entry.State == EntityState.Added)
                {
                    systemLogEntry.Event = "create";
                }
                else if (entry.State == EntityState.Modified)
                {
                    systemLogEntry.Event = "update";
                }

                entry.Entity.CreatedAt = dateTimeNow;
                entry.Entity.CreatedBy = username;

                CompanySystemLog(entry, systemLogEntry);
                EmployeeSystemLog(entry, systemLogEntry);
            }

            return await base.SaveChangesAsync();
        }

        private void EmployeeSystemLog(EntityEntry<BaseDataModel>? entry, SystemLog systemLogEntry)
        {
            if (entry?.Entity is Employee)
            {
                systemLogEntry.ResourceType = "Employee";

                var properties = entry.CurrentValues.Properties.ToList();

                systemLogEntry.Changeset += "<employee>";
                foreach (var propName in properties)
                {
                    object? current = entry.CurrentValues[propName];
                    systemLogEntry.Changeset += $"<{propName.Name}>{current}</{propName.Name}>";

                    if (propName.Name.ToLower() == "email")
                    {
                        systemLogEntry.Comment = $"new employee {current} was created";
                    }
                }

                systemLogEntry.Changeset += "</employee>";

                this.SystemLogs.Add(systemLogEntry);
            }
        }

        private void CompanySystemLog(EntityEntry<BaseDataModel>? entry, SystemLog systemLogEntry)
        {
            if (entry?.Entity is Company)
            {
                systemLogEntry.ResourceType = "Company";

                var properties = entry.CurrentValues.Properties.ToList();

                systemLogEntry.Changeset += "<company>";

                foreach (var propName in properties)
                {
                    object? current = entry.CurrentValues[propName];
                    systemLogEntry.Changeset += $"<{propName.Name}>{current}</{propName.Name}>";
                }

                systemLogEntry.Changeset += "</company>";

                systemLogEntry.Comment = $"New Company is created";

                this.SystemLogs.Add(systemLogEntry);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NpgsqlDbContext).Assembly);
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }

    }
}
