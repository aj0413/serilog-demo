# Demo of using Serilog in .Net 9


## Different ways to register Serilog as the logging provider

specific to IHostBuilder interface

```
builder.Host.UseSerilog((host, lc) => lc
    .ReadFrom.Configuration(builder.Configuration));
```

above is functionally equivalent to the following (it ultimately calls it internally), see source code:
- [IHostBuilder.UseSerilog](https://github.com/serilog/serilog-extensions-hosting/blob/87e316f7d31ae431747d1106976dfceffdecc32c/src/Serilog.Extensions.Hosting/SerilogHostBuilderExtensions.cs#L100)
- [IServiceCollection.AddSerilog](https://github.com/serilog/serilog-extensions-hosting/blob/87e316f7d31ae431747d1106976dfceffdecc32c/src/Serilog.Extensions.Hosting/SerilogServiceCollectionExtensions.cs#L129)

```
builder.Services.AddSerilog((lc) => lc
     .ReadFrom.Configuration(builder.Configuration));
```