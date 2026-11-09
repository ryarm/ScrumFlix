using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ScrumFlix.Services;

/// <summary>
/// Bounded, opt-in retention for the MySQL sink's Logs table. AuditLog is not touched.
/// </summary>
public sealed class LogRetentionService(
    IServiceScopeFactory scopeFactory,
    ILogger<LogRetentionService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromDays(1);
    private static readonly TimeSpan InitialDelay = TimeSpan.FromMinutes(5);
    private const int MaxBatches = 20;
    private const int BatchSize = 500;

    // _ts is a MySQL TIMESTAMP. NOW() uses the connection's session time zone,
    // matching MySQL's conversion of _ts; an application-side UTC cutoff may not.
    private const string DeleteBatchSql =
        "DELETE FROM `Logs` WHERE `_ts` < NOW() - INTERVAL 30 DAY " +
        "ORDER BY `_ts`, `id` LIMIT 500";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(InitialDelay, stoppingToken);
            using var timer = new PeriodicTimer(Interval);
            do
            {
                try
                {
                    var deleted = 0;
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    for (var batch = 0; batch < MaxBatches; batch++)
                    {
                        var count = await db.Database.ExecuteSqlRawAsync(
                            DeleteBatchSql, cancellationToken: stoppingToken);
                        deleted += count;
                        if (count < BatchSize) break;
                    }
                    if (deleted > 0)
                        logger.LogInformation("Removed {Count} expired application log rows.", deleted);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Application log retention sweep failed; retrying next day.");
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal host shutdown.
        }
    }
}
