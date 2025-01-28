using Microsoft.EntityFrameworkCore;
using Serilog;
using serilog_demo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((lc) => lc
    .ReadFrom.Configuration(builder.Configuration));

builder.Services.AddDbContext<WeatherForecastContext>(options => 
    options.UseSqlite($"Data Source=./weatherforecast.db")); 

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async (
    ILogger<Program> logger,
    WeatherForecastContext context,
    CancellationToken ct) =>
{
    logger.LogInformation("Test log 1..2..3");

    return await context.WeatherForecasts.ToListAsync(ct);
});

app.Run();
