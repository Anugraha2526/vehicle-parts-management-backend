using Microsoft.EntityFrameworkCore;
using vehicle_parts_management_backend.Domain.Entities;

namespace vehicle_parts_management_backend.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // table that holds all staff member records
        public DbSet<Staff> Staff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // make sure no two staff share the same email
            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Email)
                .IsUnique();
        }
    }
}
