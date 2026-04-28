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

        public DbSet<Appointment> Appointments { get; set; }
            public DbSet<Part> Parts { get; set; }
        public DbSet<PartRequest> PartRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}