using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VehicleParts.Api.Extensions;
using VehicleParts.Application.DependencyInjection;
using VehicleParts.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vehicle Parts API",
        Version = "v1",
        Description = "Vehicle Parts Selling and Inventory Management System"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JwtSettings:Key is missing.");
var keyBytes = Encoding.UTF8.GetBytes(key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        // Remove default 5-minute tolerance so tokens expire exactly on time.
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Allow all origins in development mode.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Register application services that require direct Infrastructure access.
builder.Services.AddScoped<VehicleParts.Application.Interfaces.ICustomerService, VehicleParts.Infrastructure.Services.CustomerService>();
builder.Services.AddScoped<VehicleParts.Application.Modules.CustomerPortal.Interfaces.ICustomerPortalService, VehicleParts.Infrastructure.Services.CustomerPortalService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register middleware in correct order.
app.UseErrorHandling();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var applyMigrationsOnStartup = builder.Configuration.GetValue<bool?>("StartupTasks:ApplyMigrations") ?? true;
var seedDemoDataOnStartup = builder.Configuration.GetValue<bool?>("StartupTasks:SeedDemoData")
    ?? app.Environment.IsDevelopment();

// Apply migrations and optional demo seed data.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VehicleParts.Infrastructure.Persistence.ApplicationDbContext>();

    if (applyMigrationsOnStartup)
    {
        context.Database.Migrate();
    }

    if (seedDemoDataOnStartup)
    {
        var primaryAdmin = context.Users.FirstOrDefault(u => u.Email == "admin@chitospare.com");
        if (primaryAdmin == null)
        {
            primaryAdmin = new VehicleParts.Domain.Modules.CustomerCRM.Entities.User
            {
                FullName = "Sabin Devkota",
                Email = "admin@chitospare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
                Role = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Admin,
                IsActive = true
            };
            context.Users.Add(primaryAdmin);
        }
        else
        {
            primaryAdmin.FullName = "Sabin Devkota";
            primaryAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234");
            primaryAdmin.Role = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Admin;
            primaryAdmin.IsActive = true;
        }

        var backupAdmin = context.Users.FirstOrDefault(u => u.Email == "admin@example.com");
        if (backupAdmin == null)
        {
            backupAdmin = new VehicleParts.Domain.Modules.CustomerCRM.Entities.User
            {
                FullName = "System Admin",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
                Role = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Admin,
                IsActive = true
            };
            context.Users.Add(backupAdmin);
        }
        else
        {
            backupAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234");
            backupAdmin.Role = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Admin;
            backupAdmin.IsActive = true;
        }

        var anugrahaStaff = context.Users.FirstOrDefault(u => u.Email == "anugraha@example.com");
        if (anugrahaStaff == null)
        {
            anugrahaStaff = new VehicleParts.Domain.Modules.CustomerCRM.Entities.User
            {
                FullName = "Anugraha Staff",
                Email = "anugraha@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@1234"),
                Role = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Staff,
                IsActive = true
            };
            context.Users.Add(anugrahaStaff);
        }
        else
        {
            anugrahaStaff.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@1234");
            anugrahaStaff.Role = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Staff;
            anugrahaStaff.IsActive = true;
        }

        var sampleCustomer = context.Users.FirstOrDefault(u => u.Email == "aarav@example.com");
        if (sampleCustomer == null)
        {
            sampleCustomer = new VehicleParts.Domain.Modules.CustomerCRM.Entities.User
            {
                FullName = "Aarav Sharma",
                Email = "aarav@example.com",
                PhoneNumber = "9841234567",
                Address = "Kathmandu, Nepal",
                Role = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Customer,
                IsActive = true,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@1234")
            };
            context.Users.Add(sampleCustomer);
            context.SaveChanges();

            context.Vehicles.Add(new VehicleParts.Domain.Modules.CustomerCRM.Entities.Vehicle
            {
                UserId = sampleCustomer.Id,
                VehicleNumber = "BA-1-PA-1234",
                Make = "Toyota",
                Model = "Corolla",
                Year = 2022
            });
        }
        else
        {
            sampleCustomer.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@1234");
            sampleCustomer.Role = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Customer;
            sampleCustomer.IsActive = true;
        }

        context.SaveChanges();

        if (!context.Parts.Any())
        {
            var mockVendor = new VehicleParts.Domain.Modules.AdminCore.Entities.Vendor
            {
                VendorName = "Mock Vendor",
                ContactPerson = "Mock",
                Email = "mock@mock.com",
                Phone = "1234",
                Address = "Mock City"
            };
            context.Vendors.Add(mockVendor);
            context.SaveChanges();

            var sparkPlug = new VehicleParts.Domain.Modules.AdminCore.Entities.Part
            {
                PartName = "NGK Spark Plug",
                PartNumber = "NGK-001",
                Category = "Engine",
                VendorId = mockVendor.Id,
                QuantityInStock = 100,
                UnitCost = 200,
                SellingPrice = 350
            };
            context.Parts.Add(sparkPlug);

            var brakeDisc = new VehicleParts.Domain.Modules.AdminCore.Entities.Part
            {
                PartName = "Bosch Brake Disc",
                PartNumber = "BSH-BRK",
                Category = "Brakes",
                VendorId = mockVendor.Id,
                QuantityInStock = 50,
                UnitCost = 800,
                SellingPrice = 1200
            };
            context.Parts.Add(brakeDisc);

            context.SaveChanges();
        }

        if (!context.SalesInvoices.Any(i => i.InvoiceNumber == "SINV-OVERDUE-01"))
        {
            var dummyCustomer = context.Users.FirstOrDefault(u => u.Role == VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Customer);
            var dummyStaff = context.Users.FirstOrDefault(u =>
                u.Role == VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Staff ||
                u.Role == VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Admin);

            if (dummyCustomer != null && dummyStaff != null)
            {
                var overdueInvoice = new VehicleParts.Domain.Modules.Sales.Entities.SalesInvoice
                {
                    InvoiceNumber = "SINV-OVERDUE-01",
                    CustomerId = dummyCustomer.Id,
                    StaffId = dummyStaff.Id,
                    SoldAtUtc = DateTime.UtcNow.AddDays(-40),
                    SubTotal = 6000,
                    LoyaltyDiscountApplied = true,
                    DiscountAmount = 600,
                    TotalAmount = 5400,
                    IsPaid = false
                };
                context.SalesInvoices.Add(overdueInvoice);
                context.SaveChanges();
            }
        }

        // Feature 13: Seed demo portal data so the UI can be reviewed immediately
        // Use the seeded Aarav customer as the portal demo account
        var portalCustomer = context.Users.FirstOrDefault(u => u.Email == "aarav@example.com");
        if (portalCustomer != null && !context.Appointments.Any(a => a.CustomerId == portalCustomer.Id))
        {
            context.Appointments.AddRange(
                new VehicleParts.Domain.Modules.CustomerPortal.Entities.Appointment
                {
                    CustomerId = portalCustomer.Id,
                    ServiceType = "Oil Change",
                    AppointmentAtUtc = DateTime.UtcNow.AddDays(3),
                    Status = "Confirmed",
                    Notes = "Please use 5W-30 synthetic oil."
                },
                new VehicleParts.Domain.Modules.CustomerPortal.Entities.Appointment
                {
                    CustomerId = portalCustomer.Id,
                    ServiceType = "Brake Inspection",
                    AppointmentAtUtc = DateTime.UtcNow.AddDays(10),
                    Status = "Pending",
                    Notes = string.Empty
                },
                new VehicleParts.Domain.Modules.CustomerPortal.Entities.Appointment
                {
                    CustomerId = portalCustomer.Id,
                    ServiceType = "Tire Service",
                    AppointmentAtUtc = DateTime.UtcNow.AddDays(-14),
                    Status = "Completed",
                    Notes = "Rotation and pressure check."
                }
            );
        }

        if (portalCustomer != null && !context.ServiceReviews.Any(r => r.CustomerId == portalCustomer.Id))
        {
            context.ServiceReviews.AddRange(
                new VehicleParts.Domain.Modules.CustomerPortal.Entities.ServiceReview
                {
                    CustomerId = portalCustomer.Id,
                    Rating = 5,
                    ServiceType = "Tire Service",
                    Comment = "Quick and professional. The team was very helpful and finished ahead of schedule."
                },
                new VehicleParts.Domain.Modules.CustomerPortal.Entities.ServiceReview
                {
                    CustomerId = portalCustomer.Id,
                    Rating = 4,
                    ServiceType = "Oil Change",
                    Comment = "Good service overall. Waiting area could be more comfortable."
                }
            );
        }

        if (portalCustomer != null && !context.PartRequests.Any(pr => pr.CustomerId == portalCustomer.Id))
        {
            context.PartRequests.AddRange(
                new VehicleParts.Domain.Modules.CustomerPortal.Entities.PartRequest
                {
                    CustomerId = portalCustomer.Id,
                    RequestedPartName = "Toyota Corolla 2022 Cabin Air Filter",
                    Description = "Original OEM part preferred.",
                    Status = "Approved"
                },
                new VehicleParts.Domain.Modules.CustomerPortal.Entities.PartRequest
                {
                    CustomerId = portalCustomer.Id,
                    RequestedPartName = "Bosch Windshield Wiper Blades 24-inch",
                    Description = string.Empty,
                    Status = "Pending"
                }
            );
        }

        // seed extra pending requests by name so the admin panel always has demo data
        if (portalCustomer != null && !context.PartRequests.Any(pr => pr.RequestedPartName == "Honda Civic 2021 Oil Filter"))
        {
            context.PartRequests.Add(new VehicleParts.Domain.Modules.CustomerPortal.Entities.PartRequest
            {
                CustomerId = portalCustomer.Id,
                RequestedPartName = "Honda Civic 2021 Oil Filter",
                Description = "Genuine Honda part, not aftermarket.",
                Status = "Pending"
            });
        }

        if (portalCustomer != null && !context.PartRequests.Any(pr => pr.RequestedPartName == "Denso Oxygen Sensor - Toyota"))
        {
            context.PartRequests.Add(new VehicleParts.Domain.Modules.CustomerPortal.Entities.PartRequest
            {
                CustomerId = portalCustomer.Id,
                RequestedPartName = "Denso Oxygen Sensor - Toyota",
                Description = string.Empty,
                Status = "Pending"
            });
        }

        context.SaveChanges();
    }
}

app.Run();
