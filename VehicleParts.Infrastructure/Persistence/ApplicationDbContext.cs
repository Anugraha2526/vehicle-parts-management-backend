using Microsoft.EntityFrameworkCore;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.CustomerCRM.Entities;
using VehicleParts.Domain.Modules.CustomerPortal.Entities;
using VehicleParts.Domain.Modules.Finance.Entities;
using VehicleParts.Domain.Modules.Sales.Entities;

namespace VehicleParts.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Part> Parts => Set<Part>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    // Finance module tables (schema placeholder for architecture phase).
    public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems => Set<PurchaseInvoiceItem>();
    public DbSet<LowStockNotification> LowStockNotifications => Set<LowStockNotification>();

    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<PartRequest> PartRequests => Set<PartRequest>();
    public DbSet<ServiceReview> ServiceReviews => Set<ServiceReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
