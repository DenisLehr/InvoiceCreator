using InvoiceCreator_BlazorFrontend.Components.Firmendaten.Services;
using InvoiceCreator_BlazorFrontend.Components.Kundenverwaltung.Services;
using InvoiceCreator_BlazorFrontend.Components.Leistungskatalog.Services;
using InvoiceCreator_BlazorFrontend.Components.Rechnung.Services;
using InvoiceCreator_BlazorFrontend.Components.Terminplanung.Services;
using InvoiceCreator_BlazorFrontend.Components.Userverwaltung.Services;
using InvoiceCreator_BlazorFrontend.Settings;
using Microsoft.Extensions.Options;

namespace InvoiceCreator_BlazorFrontend.Components.Common.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureFeatureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            services.AddHttpClient<KundeService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            services.AddHttpClient<LeistungService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            services.AddHttpClient<FirmaService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            services.AddHttpClient<UserService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            services.AddHttpClient<RechnungService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            services.AddHttpClient<TerminService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });
        }
    }
}
