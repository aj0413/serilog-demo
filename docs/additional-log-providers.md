# Additional Log Providers

This is a more abnormal setup, but may come up if a ready Serilog.Sink does not exist for the service you wish to send logs to, but *does* have docs on registering a log provider.

For instance, if you want to capture logs with [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/ilogger).

```
builder.Logging.AddApplicationInsights(
        configureTelemetryConfiguration: (config) => 
            config.ConnectionString = builder.Configuration.GetConnectionString("APPLICATIONINSIGHTS_CONNECTION_STRING"),
            configureApplicationInsightsLoggerOptions: (options) => { }
    );
```

In this case, you'd register Serilog via:

```
builder.Logging.AddSerilog(
    new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger()
    );
```