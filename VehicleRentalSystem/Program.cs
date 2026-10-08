using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Application.Mappings;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Application.Services;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;
using VehicleRentalSystem.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// ---------- Database ----------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VehicleRentalCS")));

// ---------- AutoMapper ----------
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

// ---------- Repositories ----------
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IAdditionalServiceRepository, AdditionalServiceRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IOfferRepository, OfferRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IComplaintRepository, ComplaintRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IBookingDriverRepository, BookingDriverRepository>();
builder.Services.AddScoped<IBookingAdditionalServiceRepository, BookingAdditionalServiceRepository>();

// ---------- Services ----------
// New repositories
builder.Services.AddScoped<IBookingDriverRepository, BookingDriverRepository>();
builder.Services.AddScoped<IBookingAdditionalServiceRepository, BookingAdditionalServiceRepository>();

// Application services (all in Application now — no Infrastructure.Services namespace)
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IAdditionalServiceService, AdditionalServiceService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificationTriggerService, NotificationTriggerService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IBookingDriverService, BookingDriverService>();
builder.Services.AddScoped<IBookingAdditionalServiceService, BookingAdditionalServiceService>();

// ---------- Controllers ----------
builder.Services.AddControllers();

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---------- Pipeline ----------
app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();