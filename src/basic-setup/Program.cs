using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using serilog_demo;

var builder = WebApplication.CreateBuilder(args);

Host.CreateApplicationBuilder();

builder.Services.AddSerilog((lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File(
        path: "./logs/log-.json",
        rollingInterval: RollingInterval.Day,
        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter()));

builder.Services.AddDbContext<WeatherForecastContext>(options => 
    options
        .UseSqlite($"Data Source=../../weatherforecast.db")
        // EnableSenstiveDataLogging isn't necessary here aside from making it so query parameter values are actually shown in logs
        .EnableSensitiveDataLogging());

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();