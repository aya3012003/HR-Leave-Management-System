using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_3.src.API.Middleware;
using Project_3.src.Application.Mapping;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Implementation;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.identity;
using Project_3.src.Infrastructure.Repositories.Implementations;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Identity
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // AutoMapper
            services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

            // Configurations
            services.Configure<JwtOptions>(configuration.GetSection("JWT"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Exception Handling
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IHolidayRepository, HolidayRepository>();
            services.AddScoped<IEmployeeLeaveBalanceRepository, EmployeeLeaveBalanceRepository>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

            // Services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHolidayService, HolidayService>();
            services.AddHttpClient<IHolidayApiService, HolidayApiService>();
            services.AddScoped<ILeaveTypeService, LeaveTypeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeLeaveBalanceService, EmployeeLeaveBalanceService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ILeaveCalculationService, LeaveCalculationService>();

            return services;
        }
    }
}
