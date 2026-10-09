
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
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
