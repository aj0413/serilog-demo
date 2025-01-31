using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace serilog_demo;

public class WeatherForecastContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options
            .UseSeeding((context, _) =>
            {
                var seed = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        Guid.CreateVersion7(),
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        Summaries[Random.Shared.Next(Summaries.Length)]
                    ));

                context.Set<WeatherForecast>().AddRange(seed);

                context.SaveChanges();
            });
    }
}

static class Extensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate) =>
        condition ? query.Where(predicate) : query;
}