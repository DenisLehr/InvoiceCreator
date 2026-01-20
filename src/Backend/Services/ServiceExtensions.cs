
using Application.Common.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using Services.Einsatzplanung;
using Services.Email;
using Services.Fakturierung;
using Services.Interfaces;
using Services.Stammdatenverwaltung;
using Shared.Contracts.Interfaces;
using Shared.Fakturierung;
using Shared.PdfGenerator;
using System.Reflection;

namespace Services
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<IEmailMusterProvider, EmailMusterProvider>();

            services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
            QuestPDF.Settings.License = LicenseType.Community;

            services.AddScoped<IFakturierungService, FakturierungService>();
            services.AddScoped<ITerminService, TerminService>();
            services.AddScoped<IRechnungsVerarbeitungsService, RechnungsVerarbeitungsService>();
            services.AddScoped<IRechnungService, RechnungService>();
            services.AddScoped<IKundeService, KundeService>();
            services.AddScoped<IFirmendatenService, FirmendatenService>();
            services.AddScoped<ILeistungService, LeistungService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
