using Microsoft.EntityFrameworkCore;
using Serilog;
using serilog_demo;

var builder = WebApplication.CreateBuilder(args);

Host.CreateApplicationBuilder();

// reads Serilog config from IConfiguration built during runtime
// will use reflection to regsiter Sinks based on config
builder.Services.AddSerilog((lc) => lc
    .ReadFrom.Configuration(builder.Configuration));

builder.Services.AddDbContext<WeatherForecastContext>(options => 
    options
        .UseSqlite($"Data Source=../../weatherforecast.db"));

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();