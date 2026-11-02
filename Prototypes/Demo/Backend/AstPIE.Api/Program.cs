using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// The connection string (with the password) is in appsettings.Local.json.
// That file is only on this computer: git ignores it.
builder.Configuration.AddJsonFile("appsettings.Local.json", true);

builder.Services.AddDbContext<AppDbContext>(ConfigureDatabase);
builder.Services.AddControllers();
builder.Services.Configure<JsonOptions>(ConfigureJson);
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Fill empty tables with the first content
using (IServiceScope scope = app.Services.CreateScope())
{
	AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	SeedData.Run(db);
}

// API description at /openapi/v1.json, only while developing
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapControllers();

app.Run();

void ConfigureDatabase(DbContextOptionsBuilder options)
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("AstPIE"));
	options.UseSnakeCaseNamingConvention();
}

// Enums are sent and read as text ("Mission"), not numbers
void ConfigureJson(JsonOptions options)
{
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}
