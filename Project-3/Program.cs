using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Project_3.src.API.Extensions;
using Project_3.src.API.Middleware;
using Project_3.src.Application.Mapping;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Implementation;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Data.Seed;
using Project_3.src.Infrastructure.identity;
using Project_3.src.Infrastructure.Repositories.Implementations;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerDocumentation();
            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));


            // JWT Options
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("JWT"));


            // Identity
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


            // Email
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddScoped<IEmailService, EmailService>();


            // Repositories
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
            builder.Services.AddScoped<IEmployeeLeaveBalanceRepository, EmployeeLeaveBalanceRepository>();
            builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();


            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IHolidayService, HolidayService>();
            builder.Services.AddHttpClient<IHolidayApiService, HolidayApiService>();
            builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEmployeeLeaveBalanceService, EmployeeLeaveBalanceService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<ILeaveCalculationService, LeaveCalculationService>();


            // Exception Handling
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();


            // Extensions
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddApiConfiguration();


            var app = builder.Build();


            app.UseExceptionHandler();


            // Seed Roles
            await app.SeedRolesAsync();


            // Seed Data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();

                await SeedData.Initialize(context, userManager);
            }


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();


            // Middleware
            app.UseRateLimiting();


            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}