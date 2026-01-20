using Data.Configuration.MongoDB;
using Data.Configuration.SMTP;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;

namespace Data
{
    public static class DataExtensions
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConfig = configuration.GetSection("MongoDB").Get<MongoSettings>();
            var mongoClient = new MongoClient(mongoConfig.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoConfig.DatabaseName);
            services.AddSingleton(mongoDatabase);

            var emailConfig = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
            services.AddSingleton(emailConfig);
            

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFirmaRepository, FirmaRepository>();
            services.AddScoped<IKundeRepository, KundeRepository>();
            services.AddScoped<ILeistungRepository, LeistungRepository>();
            services.AddScoped<IRechnungRepository, RechnungRepository>();
            services.AddScoped<ITerminRepository, TerminRepository>();

        }
    }
}
