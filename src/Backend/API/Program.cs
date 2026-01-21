using API.Extension;
using Data;
using Data.Persistence.Seeding;
using Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var connStr = builder.Configuration.GetValue<string>("MongoDB:ConnectionString");
Console.WriteLine($"Mongo Connection String: {connStr}");

// Add services to the container.
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureCorsPolicy();
builder.Services.ConfigureApiBehavior();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddLogging();

builder.Host.ConfigureSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); ;

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.SwaggerConfig(builder.Configuration, "SwaggerConfigTest");
}

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var enabled = config.GetValue<bool>("DatabaseSeeding:Enabled");

    if (enabled)
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();
    }
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseErrorHandler();
app.MapControllers();

app.Run();
