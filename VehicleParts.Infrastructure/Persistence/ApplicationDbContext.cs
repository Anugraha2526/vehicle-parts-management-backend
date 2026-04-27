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

        modelBuilder.Entity<PurchaseInvoice>(builder =>
        {
            builder.Property(invoice => invoice.InvoiceNumber)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(invoice => invoice.TotalAmount)
                .HasPrecision(18, 2);

            builder.HasMany(invoice => invoice.Items)
                .WithOne(item => item.PurchaseInvoice)
                .HasForeignKey(item => item.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PurchaseInvoiceItem>(builder =>
        {
            builder.Property(item => item.UnitCost)
                .HasPrecision(18, 2);

            builder.HasIndex(item => item.PartId);
        });

        modelBuilder.Entity<LowStockNotification>(builder =>
        {
            builder.Property(alert => alert.PartName)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(alert => new { alert.PartId, alert.IsAcknowledged });
        });

        modelBuilder.Entity<SalesInvoice>(builder =>
        {
            builder.Property(invoice => invoice.TotalAmount)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Part>(builder =>
        {
            builder.Property(part => part.UnitPrice)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Vendor>(builder =>
        {
            builder.HasIndex(v => v.Email).IsUnique();
            builder.Property(v => v.VendorName).HasMaxLength(200).IsRequired();
            builder.Property(v => v.Email).HasMaxLength(150).IsRequired();
            builder.Property(v => v.ContactPerson).HasMaxLength(100).IsRequired();
            builder.Property(v => v.Phone).HasMaxLength(20).IsRequired();
            builder.Property(v => v.Address).HasMaxLength(500);
            builder.Property(v => v.Notes).HasMaxLength(500);
        });
    }
}
