using Microsoft.OpenApi;
using System.Reflection;



namespace Project_3.src.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1",
                    Description = "A comprehensive REST API for managing products and orders",
                    Contact = new OpenApiContact
                    {
                        Name = "GBG Academy",
                        Email = "api@gbg.com"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                const string schemeId = "Bearer";

                c.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });

                c.AddSecurityRequirement(document =>
                {
                    var requirement = new OpenApiSecurityRequirement();

                   
                    var schemeReference = new OpenApiSecuritySchemeReference(schemeId, document);

                    requirement.Add(schemeReference, new List<string>());

                    return requirement;
                });
            });

            return services;
        }
    }
}