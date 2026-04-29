using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VehicleParts.Api.Extensions;
using VehicleParts.Application.DependencyInjection;
using VehicleParts.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
        // remove default 5-minute tolerance so tokens expire exactly on time
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// allow all origins in development mode
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Register Application Services
builder.Services.AddScoped<VehicleParts.Application.Interfaces.ICustomerService, VehicleParts.Infrastructure.Services.CustomerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// register middleware in correct order
app.UseErrorHandling();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed Sample Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VehicleParts.Infrastructure.Persistence.ApplicationDbContext>();
    context.Database.EnsureCreated();

    if (!context.Users.Any(u => u.Role == "Customer"))
    {
        var sampleCustomer = new VehicleParts.Domain.Modules.CustomerCRM.Entities.User
        {
            FullName = "Aarav Sharma",
            Email = "aarav@example.com",
            PhoneNumber = "9841234567",
            Address = "Kathmandu, Nepal",
            Role = "Customer",
            IsActive = true
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
        context.SaveChanges();
    }
}

app.Run();
