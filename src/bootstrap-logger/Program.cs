using Microsoft.EntityFrameworkCore;
using Serilog;
using serilog_demo;

// creates initial boostrap logger for use, before access to configuration or services
// will be reconfigured based on Services.AddSerilog, unless specified to be preserved
// this is a 'short' lived logger meant to be used via Serilog.Log before other configuration loads
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(
        outputTemplate: "Bootstrap Logger: [{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        theme: CustomThemes.Theme())
    .WriteTo.File(
        path: "./logs/bootstrap/log-.log",
        rollingInterval: RollingInterval.Day,
        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter())
    .CreateBootstrapLogger();

try {
    Log.Information("Application starting up...");

    var builder = WebApplication.CreateBuilder(args);
    
    // will output via initial bootstrap logger
    // throw new Exception("error before host is built");

    Host.CreateApplicationBuilder();

    builder.Services.AddSerilog((lc) => lc
        .ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddDbContext<WeatherForecastContext>(options => 
        options
            .UseSqlite($"Data Source=../../weatherforecast.db"));

    var app = builder.Build();

    // will output via configured console logger
    // throw new Exception("error after host is built");

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.MapEndpoints();

    app.Run();
}
catch (Exception exception) {
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally {
    // appears inside of ./logs/log-.json
    // does NOT appear in ./logs/bootstrap/log-.log
    Log.Information("Closing and flushing logger");
    Log.CloseAndFlush();
}
