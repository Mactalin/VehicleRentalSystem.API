using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using VehicleRentalSystem.Application.Common;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Application.Mapper;
using VehicleRentalSystem.Application.Services;
using VehicleRentalSystem.Authorization;
using VehicleRentalSystem.BackgroundServices;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;
using VehicleRentalSystem.Infrastructure.Data.Seed;
using VehicleRentalSystem.Infrastructure.Notifications;
using VehicleRentalSystem.Infrastructure.Repository;
using VehicleRentalSystem.Infrastructure.Security;
using VehicleRentalSystem.JsonConverters;
using VehicleRentalSystem.Middleware;


namespace VehicleRentalSystem
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- Database ----------
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("VehicleRentalCS")));

            // ---------- Settings ----------
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddSingleton(
                builder.Configuration.GetSection("Billing").Get<BillingSettings>() ?? new BillingSettings());
            builder.Services.AddSingleton<BranchClock>();

            // ---------- AutoMapper (one registration for both teammates' profiles) ----------
            var mapperAssemblies = new[]
            {
                typeof(MappingProfile).Assembly,
                typeof(CustomerMappingProfile).Assembly
            }.Distinct().ToArray();

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"];
                cfg.AddMaps(mapperAssemblies);
            });

            // ---------- Repositories ----------
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Anbu
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

            // Mac
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IKycRepository, KycRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IPricingRepository, PricingRepository>();
            builder.Services.AddScoped<IBranchLookupRepository, BranchLookupRepository>();
            builder.Services.AddScoped<IVehicleSearchRepository, VehicleSearchRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ============ LENIN: Repositories (Modules 9-12) ============
            builder.Services.AddScoped<IDriverRepository, DriverRepository>();
            builder.Services.AddScoped<IAdditionalServiceRepository, AdditionalServiceRepository>();
            builder.Services.AddScoped<IBookingDriverRepository, BookingDriverRepository>();
            builder.Services.AddScoped<IBookingAdditionalServiceRepository, BookingAdditionalServiceRepository>();
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();
            builder.Services.AddScoped<IOfferRepository, OfferRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IComplaintRepository, ComplaintRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            // ===========================================================


            // ---------- Services ----------
            // Anbu
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IVehicleService, VehicleService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IPricingService, PricingService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();

            // Mac
            builder.Services.AddScoped<IPriceCalculator, PriceCalculator>();
            builder.Services.AddScoped<IPaymentGateway, MockPaymentGateway>();   // swap for Razorpay/Stripe later
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IVehicleSearchService, VehicleSearchService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();


            // ============ LENIN: Application services ============
            builder.Services.AddScoped<IDriverService, DriverService>();
            builder.Services.AddScoped<IAdditionalServiceService, AdditionalServiceService>();
            builder.Services.AddScoped<IBookingDriverService, BookingDriverService>();
            builder.Services.AddScoped<IBookingAdditionalServiceService, BookingAdditionalServiceService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddScoped<IOfferService, OfferService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IComplaintService, ComplaintService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IReportService, ReportService>();

            // ============ LENIN: Notification pipeline ============
            builder.Services.AddScoped<INotificationChannel, EmailNotificationChannel>();
            builder.Services.AddScoped<INotificationChannel, SmsNotificationChannel>();
            builder.Services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
            builder.Services.AddScoped<INotificationTriggerService, NotificationTriggerService>();
            // ======================================================


            // ---------- Background jobs ----------
            builder.Services.AddHostedService<BookingExpiryWorker>();

            // ---------- Authentication (Anbu) ----------
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

            // ---------- Controllers and JSON ----------
            builder.Services
                .AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());   // "Confirmed" instead of 2
                    o.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());      // always UTC with a trailing Z
                    o.JsonSerializerOptions.Converters.Add(new UtcNullableDateTimeConverter());
                });

            builder.Services.AddHttpContextAccessor();   // ← required by Lenin's controllers

            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            builder.Services.AddOpenApi();

            var app = builder.Build();

            // ---------- Development only: create the database and seed test data ----------
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
                DbSeeder.SeedAsync(db).GetAwaiter().GetResult();
                //DevDataSeeder.SeedAsync(db).GetAwaiter().GetResult();
            }

            // ---------- Pipeline ----------
            app.UseMiddleware<ExceptionHandlingMiddleware>();    // first, so it catches everything below

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