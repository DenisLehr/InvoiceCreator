using API.Extension;
using Data;
using Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseErrorHandler();
app.MapControllers();

app.Run();
