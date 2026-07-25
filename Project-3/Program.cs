using Microsoft.AspNetCore.Identity;
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
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace Project_3
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddControllers();
        
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("JWT"));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
            builder.Services.AddScoped<IEmployeeLeaveBalanceRepository, EmployeeLeaveBalanceRepository>();
            builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
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
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            //extensions
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddApiConfiguration();
            builder.Services.AddSwaggerDocumentation();



            var app = builder.Build();

            app.UseExceptionHandler();

            await app.SeedRolesAsync();

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
            //middleware
            app.UseRateLimiting();
         
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}