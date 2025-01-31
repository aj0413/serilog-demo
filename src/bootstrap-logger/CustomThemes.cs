using Serilog.Sinks.SystemConsole.Themes;

namespace serilog_demo;

public static class CustomThemes
{
    public static SystemConsoleTheme Theme()
    {
        Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle> styles = new()
            {
                {
                    ConsoleThemeStyle.Text, new()
                    {
                        Foreground = ConsoleColor.Red,
                    }
                },
                {
                    ConsoleThemeStyle.TertiaryText, new()
                    {
                        Foreground = ConsoleColor.Yellow,
                    }
                },
            };

        return new SystemConsoleTheme(styles);
    }
}