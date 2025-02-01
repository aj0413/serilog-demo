# Demo(s) of using Serilog in .Net 9

This repository is to act as a learning tool and demo for Serilog and logging in general.

This should (hopefully) cover most normal topics and use-cases.

There's a few helpful demo projects under `src/*`; each should be aptly named towards what they're trying to teach.

Additionally, there's a write-ups of what each demo is trying to teach under `docs/*`, plus some extra.

## What's covered so far?

- [How does logging work?](docs/how-logging-works.md)
- [Structured vs Unstructured Logs](docs/structured-vs-unstructured-logs.md)
- [Log Enrichment](docs/log-enrichment.md)
- [Log Filtering](docs/log-filtering.md)
- [Basic Setup](docs/basic-setup.md#basic-setup)
- [Appsettings Setup](docs/appsettings-setup.md#appsettings-setup)
- [Boostrap Logger](docs/boostrap-logger.md#boostrap-logger)
- [Custom Serilog Sinks](docs/custom-serilog-sinks.md)
- [Conditional Serilog Sinks](docs/conditional-serilog-sinks.md)
- [Additional Log Providers](docs/additional-log-providers.md#additional-log-providers)

## Running the demos

Everything under `src` is using the `Directory.Build.props` for universal config. The individual `*.csproj` files will only hold stuff specific to a demo.

All projects make use of the `./weatherforecast.db` SQLite database file to act as backend for EF Core; shouldn't need to run migrations as the current saved file state is pre-seeded.

There's a `serilog-demo.http` file for REST Client to make calls to endpoint(s).

`.vscode/launch.json` comes pre-configured for running each demo using the `https` launchSettingsProfile.

Color themes may or may not show depending on terminal you're outputting to; VS Code integrated Debug Console will ignore Serilog config for instance.

## Need more help?

If there's something that docs+demos can't help you figure out or understand, do open an issue.

## Contributions

If you want to open a PR to add to this, feel free. This is going to be a sort-of 'living document' as it were.

I think it'd be cool to have others add to it over time; just ask you follow the template stamped out, if you can.