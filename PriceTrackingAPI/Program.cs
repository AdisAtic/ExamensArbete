// Program.cs
using System;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PriceTrackingAPI;
using PriceTrackingAPI.Configurations;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Services;
using PriceTrackingAPI.Services.FeedParser;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------
// Serilog - 
// -----------------------------------------------------
// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Debug()
//     .WriteTo.Console()
//     .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
//     .CreateLogger();
// builder.Host.UseSerilog();

// -----------------------------------------------------
// CORS (för React Dev Server)
// -----------------------------------------------------
builder.Services.AddCors(o => o.AddPolicy("AllowReact", p =>
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader()
     .AllowAnyMethod()
));

// -----------------------------------------------------
// Entity Framework / DbContext
// -----------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// -----------------------------------------------------
// Options‐binding + validering
// -----------------------------------------------------
// 1) PriceAgentOptions (API‐klient)
//  builder.Services.Configure<PriceAgentOptions>(
//      builder.Configuration.GetSection("ApiSettings:PriceAgent"));
//  – kan valideras med DataAnnotations om du vill
builder.Services.AddOptions<PriceAgentOptions>()
    .Bind(builder.Configuration.GetSection("ApiSettings:PriceAgent"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// 2) PriceUpdateSettings (schema för feed/parsing)
builder.Services.AddPriceUpdateOptions(builder.Configuration);

// 3) LitiumSettings (service‐account + schemaläggning)
builder.Services.AddOptions<LitiumSettings>()
    .Bind(builder.Configuration.GetSection("LitiumSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// -----------------------------------------------------
// Typed HttpClient för din FeedParser
// -----------------------------------------------------
builder.Services.AddHttpClient<IFeedParser, FeedParser>((sp, client) =>
{
    var opts = sp.GetRequiredService<IOptions<PriceAgentOptions>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl);

    var auth = opts.Authentication;
    switch (auth.Method)
    {
        case AuthenticationMethod.Basic:
            var cred = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{auth.Username}:{auth.Password}"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", cred);
            break;

        case AuthenticationMethod.Bearer:
        case AuthenticationMethod.ServiceAccount:
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", auth.Token!);
            break;

        case AuthenticationMethod.ClientCredentials:
            // TODO: lägg in din DelegatingHandler för ClientCredentials
            break;
    }
});

// -----------------------------------------------------
// Applikationstjänster
// -----------------------------------------------------
builder.Services.AddScoped<IPriceUpdateService, PriceUpdateService>();
builder.Services.AddScoped<IProductPriceService, ProductPriceService>();
if (!builder.Environment.IsEnvironment("Migration"))
    builder.Services.AddHostedService<LitiumPriceUpdaterService>();

// -----------------------------------------------------
// Controllers & Swagger
// -----------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -----------------------------------------------------
// Middleware‐pipeline
// -----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
