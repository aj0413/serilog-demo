using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using serilog_demo;

var builder = WebApplication.CreateBuilder(args);

Host.CreateApplicationBuilder();

// registers Serilog as only log provider and outputs to file ('./logs/log-{day}.log') and console
builder.Services.AddSerilog((lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File(
        path: "./logs/log-.log",
        rollingInterval: RollingInterval.Day,
        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter()));

builder.Services.AddDbContext<WeatherForecastContext>(options => 
    options
        .UseSqlite($"Data Source=../../weatherforecast.db"));

var app = builder.Build();

// adds middleware for streamlined request logging
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();