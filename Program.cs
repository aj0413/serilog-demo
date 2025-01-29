using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using serilog_demo;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((lc) => lc
        .ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddDbContext<WeatherForecastContext>(options => 
        options.UseSqlite($"Data Source=./weatherforecast.db").EnableSensitiveDataLogging()); 

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.MapGet("/weatherforecast", async (
        string? summary,
        int? temperatureC,
        ILogger<Program> logger,
        WeatherForecastContext context,
        CancellationToken ct) =>
    {
        logger.LogInformation("Test log 1..2..3");

        var query = context.WeatherForecasts.WhereIf(!string.IsNullOrWhiteSpace(summary), w => w.Summary == summary);
        query = query.WhereIf(temperatureC is int, w => w.TemperatureC >= temperatureC);

        return await query.ToListAsync(ct);
    });

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static class Extensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate) =>
        condition ? query.Where(predicate) : query;
}
