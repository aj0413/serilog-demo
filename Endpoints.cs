using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;

namespace serilog_demo;

public static class RegisterEndpoints
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/weatherforecast", async (
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

        routeBuilder.MapPost("/throw-error", () =>
        {
            throw new Exception("Exception handling test...");
        });

        routeBuilder.MapPost("/test-loggers", (
            ILogger<Program> normalLogger,
            Serilog.ILogger serilogAbstractLogger,
            // ILoggerFactory registered by Serilog as SerilogLoggerFactory to handle all logs
            // Replaces the default implementation, thus losing all other log providers registered normally, unless writeToProviders: true
            ILoggerFactory loggerFactory) =>
        {
            using var ctx = LogContext.PushProperty("TestLog", true);

            normalLogger.LogInformation("DI => ILogger<Program> runtime type: {type}", normalLogger.GetType());

            var factoryLogger = loggerFactory.CreateLogger<Program>();

            factoryLogger.LogInformation("ILoggerFactory.Create<Program> => ILogger<Program> runtime type: {type}", factoryLogger.GetType());

            serilogAbstractLogger.Information("DI => Serilog.ILogger runtime type: {type}", serilogAbstractLogger.GetType());

            // This grabs the static/singleton Serilog.Log instance
            // See how the type changes based on preserveStaticLogger
            var staticLogger = Log.ForContext<Program>();

            staticLogger.Information("Log.ForContext<Program>() => Serilog.ILogger runtime type: {type}", staticLogger.GetType());
        });

        return routeBuilder;
    }
}