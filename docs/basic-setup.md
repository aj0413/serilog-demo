# Basic Setup

This is the normal and intended use-case for serilog. Generally speaking, you should be using Serilog.Sinks to log to different places as their equivalent.

Confusingly, this two ways to do this at the moment.

If you're working in web app and are using  `WebApplication.CreateBuilder` then you have access to `IHostBuilder` via `builder.Host`:

```
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, lc) => lc
    .ReadFrom.Configuration(builder.Configuration));
```

If you *don't* have access to `IHostBuilder`, such as a non-web app, then you need to do the following:

```
var builder = Host.CreateApplicationBuilder();

builder.Services.AddSerilog((lc) => lc
     .ReadFrom.Configuration(builder.Configuration));
```

Note, however, that the latter is actually an extension on `IServiceCollection` and can thus be called in both cases. Furthermore, the first example is just a thin wrapper over it. 

This is why it's the new default in the documentation/examples.

See:
- [GitHub Issue: UseSerilog support for .net 7 CreateApplicationBuilder and HostApplicationBuilder](https://github.com/serilog/serilog/issues/1855)
- [GitHub Issue: UseSerilog support for .net8 IHostApplicationBuilder](https://github.com/serilog/serilog-extensions-hosting/issues/76)

Source code:
- [IHostBuilder.UseSerilog](https://github.com/serilog/serilog-extensions-hosting/blob/87e316f7d31ae431747d1106976dfceffdecc32c/src/Serilog.Extensions.Hosting/SerilogHostBuilderExtensions.cs#L100)
- [IServiceCollection.AddSerilog](https://github.com/serilog/serilog-extensions-hosting/blob/87e316f7d31ae431747d1106976dfceffdecc32c/src/Serilog.Extensions.Hosting/SerilogServiceCollectionExtensions.cs#L129)