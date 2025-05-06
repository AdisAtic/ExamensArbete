using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PriceTrackingAPI.Configurations;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Services;
using PriceTrackingAPI.Services.FeedParser;
using Serilog;

// Konfigurera Serilog globalt
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Application is starting...");

    var builder = WebApplication.CreateBuilder(args);

    // --- Konfigurera host-URLs (valfritt, om du vill styra portar) ---
    // builder.WebHost.UseUrls("http://localhost:5089", "https://localhost:7047");

    // --- Lägg till CORS-tjänst för React ---
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReact", policy =>
            policy
                .WithOrigins("http://localhost:5173")  // React-dev-server
                .AllowAnyHeader()
                .AllowAnyMethod()
        );
    });

    // --- Konfigurera DbContext (PostgreSQL) ---
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // --- Konfigurera LitiumSettings från appsettings.json ---
    builder.Services.Configure<PriceUpdateSettings>(builder.Configuration.GetSection("PriceUpdateSettings"));
    builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<PriceUpdateSettings>>().Value);

    // --- Registrera HTTP-klient och din PriceUpdateService ---
    builder.Services.AddHttpClient();  // för att injicera HttpClient i PriceUpdateService
    builder.Services.AddScoped<IPriceUpdateService, PriceUpdateService>();
    builder.Services.AddScoped<IProductPriceService, ProductPriceService>();
    builder.Services.AddScoped<IFeedParser, FeedParser>();



    // --- Lägg till bakgrundstjänst om inte i Migrations-miljö ---
    if (!builder.Environment.IsEnvironment("Migration"))
    {
        builder.Services.AddHostedService<LitiumPriceUpdaterService>();
    }

    // --- Controllers + Swagger ---
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // --- Middleware ---
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // --- Aktivera CORS-policyn tidigt i pipelinen ---
    app.UseCors("AllowReact");

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start!");
}
finally
{
    Log.CloseAndFlush();
}
