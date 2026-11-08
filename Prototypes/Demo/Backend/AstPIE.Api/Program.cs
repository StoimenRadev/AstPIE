using System;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;
using AstPIE.Api.Security;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// The connection string and the admin key are in appsettings.Local.json.
// That file is only on this computer: git ignores it.
builder.Configuration.AddJsonFile("appsettings.Local.json", true);
// Environment variables and the command line still win over that file (for example to test on another schema)
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services.AddDbContext<AppDbContext>(ConfigureDatabase);
builder.Services.AddControllers();
builder.Services.Configure<JsonOptions>(ConfigureJson);
builder.Services.AddOpenApi();

// Admin: one secret key, checked on every admin request
builder.Services.AddAuthentication(AdminKeyAuthentication.SchemeName)
	.AddScheme<AuthenticationSchemeOptions, AdminKeyAuthentication>(AdminKeyAuthentication.SchemeName, null);
builder.Services.AddAuthorization(ConfigureAuthorization);
builder.Services.AddRateLimiter(ConfigureRateLimiter);

// Uploaded files can be up to 100 MB (videos)
builder.Services.Configure<FormOptions>(ConfigureUploads);

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

// Uploaded media files are served from wwwroot/uploads
app.UseStaticFiles();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

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

void ConfigureAuthorization(AuthorizationOptions options)
{
	options.AddPolicy(AdminKeyAuthentication.PolicyName, ConfigureAdminPolicy);
}

void ConfigureAdminPolicy(AuthorizationPolicyBuilder policy)
{
	policy.AddAuthenticationSchemes(AdminKeyAuthentication.SchemeName);
	policy.RequireRole("Admin");
}

// At most 120 admin requests per minute: plenty for working in the admin pages,
// far too few to guess the key
void ConfigureRateLimiter(RateLimiterOptions options)
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.AddFixedWindowLimiter("admin", ConfigureAdminLimit);
}

void ConfigureAdminLimit(FixedWindowRateLimiterOptions limit)
{
	limit.PermitLimit = 120;
	limit.Window = TimeSpan.FromMinutes(1);
	limit.QueueLimit = 0;
}

void ConfigureUploads(FormOptions options)
{
	options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
}
