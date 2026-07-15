using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_3.src.API.Extensions;
using Project_3.src.Application.Interfaces.IServices;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Data.Seed;
using Project_3.src.Infrastructure.identity;

namespace Project_3
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("JWT"));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            var app = builder.Build();

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}