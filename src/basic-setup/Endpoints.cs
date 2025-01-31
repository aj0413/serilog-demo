using Microsoft.EntityFrameworkCore;

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

        return routeBuilder;
    }
}