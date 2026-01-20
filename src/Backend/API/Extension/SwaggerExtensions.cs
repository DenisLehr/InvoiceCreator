using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace API.Extension
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                
                options.IncludeXmlComments(xmlPath);

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = configuration["Swagger:Title"] ?? "Leistungserfassung REST-API",
                    Version = "v1",
                    Description = configuration["Swagger:Description"] ?? "API Dokumentation für mobile Leistungserfassung",
                    Contact = new OpenApiContact
                    {
                        Name = "Ciblu-software GmbH",
                        Email = "info@ciblu.eu",
                        Url = new Uri("https://www.ciblu.eu/")
                    }
                });
                options.UseInlineDefinitionsForEnums();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
        public static void SwaggerConfig(this IApplicationBuilder app, IConfiguration configuration, string swaggerConfigName = "SwaggerConfig")
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string endPoint = configuration[$"{swaggerConfigName}:EndPoint"] ?? "/swagger/v1/swagger.json";
                string title = configuration[$"{swaggerConfigName}:Title"] ?? "API Documentation";

                c.SwaggerEndpoint(endPoint, title);
                c.DocumentTitle = $"{title} Documentation";
                c.DocExpansion(DocExpansion.List);
            });
        }
    }
}
