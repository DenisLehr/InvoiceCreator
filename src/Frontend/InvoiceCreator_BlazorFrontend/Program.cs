using InvoiceCreator_BlazorFrontend.Components;
using InvoiceCreator_BlazorFrontend.Components.Common.Extensions;
using InvoiceCreator_BlazorFrontend.Components.Firmendaten.Services;
using InvoiceCreator_BlazorFrontend.Components.Kundenverwaltung.Services;
using InvoiceCreator_BlazorFrontend.Components.Leistungskatalog.Services;
using InvoiceCreator_BlazorFrontend.Components.Login.Services;
using InvoiceCreator_BlazorFrontend.Components.Rechnung.Services;
using InvoiceCreator_BlazorFrontend.Components.Terminplanung.Services;
using InvoiceCreator_BlazorFrontend.Components.Userverwaltung.Services;
using InvoiceCreator_BlazorFrontend.Settings;
using Microsoft.Extensions.Options;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<KundeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FirmaService>();
builder.Services.AddScoped<TerminService>();
builder.Services.AddScoped<RechnungService>();
builder.Services.AddScoped<LeistungService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<NotificationService>();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.AddHttpClient<AuthService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<AuthSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});

builder.Services.ConfigureFeatureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
