using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Application.Mapper;
using VehicleRentalSystem.Application.Services;
using VehicleRentalSystem.Authorization;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;
using VehicleRentalSystem.Infrastructure.Data.Seed;
using VehicleRentalSystem.Infrastructure.Repository;
using VehicleRentalSystem.Infrastructure.Security;
using VehicleRentalSystem.Middleware;

using System.Text.Json.Serialization;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Application.Mapper;
using VehicleRentalSystem.Application.Services;
using VehicleRentalSystem.BackgroundServices;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Repository;
using VehicleRentalSystem.JsonConverters;

namespace VehicleRentalSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // ---------- AutoMapper ----------
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"];
                cfg.AddMaps(typeof(CustomerMappingProfile).Assembly);
            });
            // ---------- Repositories (Infrastructure) ----------
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IKycRepository, KycRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // ---------- Services (Application) ----------
            builder.Services.AddScoped<IPriceCalculator, PriceCalculator>();
            builder.Services.AddScoped<IPaymentGateway, MockPaymentGateway>();   // swap for Razorpay/Stripe later
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IVehicleSearchService, VehicleSearchService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();

            // ---------- Background jobs ----------
            builder.Services.AddHostedService<BookingExpiryWorker>();


            builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));


            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();


            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();


            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IVehicleService, VehicleService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IPricingService, PricingService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();


            var jwt = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwt["Key"];
            if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
                throw new InvalidOperationException("Jwt:Key must be configured and at least 32 characters long.");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name
                    };
                });


            builder.Services.AddAuthorizationBuilder()
                   .AddPolicy(Policies.AdminOnly, p => p.RequireRole("Admin"))
                   .AddPolicy(Policies.AdminOrManager, p => p.RequireRole("Admin", "Manager"))
                   .AddPolicy(Policies.StaffAndAbove, p => p.RequireRole("Admin", "Manager", "Staff"));
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());     // "Confirmed" instead of 2
        o.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());        // always UTC with a trailing Z
        o.JsonSerializerOptions.Converters.Add(new UtcNullableDateTimeConverter());
    });
           builder.Services.AddCors(options =>
                     options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();


            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //    db.Database.Migrate();
            //    DbSeeder.SeedAsync(db).GetAwaiter().GetResult();
            //}


            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
