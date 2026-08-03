using Project_3.src.API.Extensions;
using Project_3.src.API.Middleware;
using Serilog;

namespace Project_3
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Logging
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            // Core API Services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerDocumentation();

            // Application Services via Extension Methods
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddApiConfiguration();

            var app = builder.Build();

            // HTTP Request Pipeline
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("Development");
            app.UseHttpsRedirection();

            // Middlewares
            app.UseRateLimiting();
            app.UseRequestTiming();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Database seeding
            await app.SeedDatabaseAsync();

            app.Run();
        }
    }
}

// Preserved for integration testing
public partial class Program { }