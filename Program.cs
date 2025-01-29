using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using serilog_demo;

// Creates initial ReloadableLogger for use before access to configuration or services
// Will be reconfigured later, unless specified to be preserved
// This is a 'short' lived logger meant to be used via Serilog.Log before other configuration loads
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: "./logs/bootstrap/log-.json",
        rollingInterval: RollingInterval.Day,
        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter())
    .CreateBootstrapLogger();

// Diagnose Serilog itself
Serilog.Debugging.SelfLog.Enable(Console.Error);

try
{
    Log.Information("Application starting up...");

    var builder = WebApplication.CreateBuilder(args);

    Host.CreateApplicationBuilder();

    // Alternative way to register Serilog as only provider; acts as thin wrapper for below method
    // Only works with IHostBuilder which is unique to web applications
    // builder.Host.UseSerilog((host, lc) => lc
    //     .ReadFrom.Configuration(builder.Configuration));

    // Prefered way to register Serilog as only provider
    // Works with all application/builder models
    builder.Services.AddSerilog((lc) => lc
        .ReadFrom.Configuration(builder.Configuration),
        // Set to true to see Serilog.Log continue to use settings set above
        // Othewise, Serilog.Log will be reconfigured based on these settings
        preserveStaticLogger: false,
        // Set to true to see Serilog preserve the other log providers registered via builder.Logging
        //  Otherwise, Serilog will ignore all other providers
        writeToProviders: false);

    var efLogger = new LoggerConfiguration()
                    .WriteTo.File(
                        path: "./logs/ef-core/log-.json",
                        rollingInterval: RollingInterval.Day,
                        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter())
                    .CreateLogger();

    builder.Services.AddDbContext<WeatherForecastContext>(options => 
        options
            .UseSqlite($"Data Source=./weatherforecast.db")
            // EnableSenstiveDataLogging isn't necessary here aside from making it query parameter values are actually shown in logs
            .EnableSensitiveDataLogging()
            // Using created logger to capture logs from EF Core; unnecessary for the most part, however
            .LogTo(msg => efLogger.Information(msg))); 

    // Adds Serilog along with other log providers
    // Notice logs will still go to console (add be formatted differently) because you're now using the default log providers+Serilog
    // builder.Logging
    //     .AddSerilog(
    //         new LoggerConfiguration()
    //             .WriteTo.File(
    //                 path: "./logs/another-provider/log-.json",
    //                 rollingInterval: RollingInterval.Day,
    //                 formatter: new Serilog.Formatting.Compact.CompactJsonFormatter())
    //             .CreateLogger()
    //         );

    var app = builder.Build();

    // Comment out if testing builder.Logging.AddSerilog as the required middleware will be missing
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.MapGet("/weatherforecast", async (
        string? summary,
        int? temperatureC,
        ILogger<Program> logger,
        WeatherForecastContext context,
        CancellationToken ct) =>
    {
        logger.LogInformation("Calling endpoint for weather stuffs");

        var query = context.WeatherForecasts.WhereIf(!string.IsNullOrWhiteSpace(summary), w => w.Summary == summary);
        query = query.WhereIf(temperatureC is int, w => w.TemperatureC >= temperatureC);

        return await query.ToListAsync(ct);
    });

    app.MapPost("/throw-error", () =>
    {
        throw new Exception("Exception handling test...");
    });

    app.MapPost("/test-loggers", (
        ILogger<Program> normalLogger,
        Serilog.ILogger serilogAbstractLogger,
        // ILoggerFactory registered by Serilog as SerilogLoggerFactory to handle all logs
        // Replaces the default implementation, thus losing all other log providers registered normally, unless writeToProviders: true
        ILoggerFactory loggerFactory) =>
    {
        normalLogger.LogInformation("DI => ILogger<Program> runtime type: {type}", normalLogger.GetType());

        var factoryLogger = loggerFactory.CreateLogger<Program>();

        factoryLogger.LogInformation("ILoggerFactory.Create<Program> => ILogger<Program> runtime type: {type}", factoryLogger.GetType());

        serilogAbstractLogger.Information("DI => Serilog.ILogger runtime type: {type}", serilogAbstractLogger.GetType());

        // This grabs the static/singleton Serilog.Log instance
        // See how the type changes based on preserveStaticLogger
        var staticLogger = Log.ForContext<Program>();

        staticLogger.Information("Log.ForContext<Program>() => Serilog.ILogger runtime type: {type}", staticLogger.GetType());
    });

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    // Notice that this log ends up going with the rest of the logs instead of inside of ./logs/bootstrap/log-.json
    // Will use configuration for intial BoostrapLogger if preserveStaticLogger: true
    Log.Information("Closing and flushing logger");
    Log.CloseAndFlush();
}

static class Extensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate) =>
        condition ? query.Where(predicate) : query;
}
