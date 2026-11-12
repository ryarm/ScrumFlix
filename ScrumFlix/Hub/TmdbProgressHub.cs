/*
 * File:      /ScrumFlix/Hub/TmdbProgressHub.cs
 * Namespace: ScrumFlix.Hubs
 * Purpose: Legacy TMDb-specific SignalR hub retained at /tmdbSyncHub.
 *          It joins connections to the "tmdb-sync" group but the current
 *          AdminHomeController action publishes progress through the shared
 *          ProgressHub at /progressHub instead. The June 2026 session log
 *          predates the shared queue/progress implementation.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ScrumFlix.Services;

namespace ScrumFlix.Hubs;

/// <summary>
/// Legacy TMDb-specific hub; the active bulk-sync progress path uses
/// ProgressHub. Kept mapped for compatibility until deliberately removed.
/// </summary>
[Authorize(Policy = "AdminStaff")]
public class TmdbProgressHub : Hub
{
    private readonly ILogger<TmdbProgressHub> _logger;

    /// <summary>Group name for this legacy hub.</summary>
    public const string SyncGroup = "tmdb-sync";

    public TmdbProgressHub(ILogger<TmdbProgressHub> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        // Retain legacy group membership for any client that still connects here.
        await Groups.AddToGroupAsync(Context.ConnectionId, SyncGroup);

        _logger.LogDebug(
            "TmdbProgressHub: client connected — ConnectionId={ConnectionId}",
            Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    /// <inheritdoc />
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is not null)
        {
            _logger.LogWarning(exception,
                "TmdbProgressHub: client disconnected with error — ConnectionId={ConnectionId}",
                Context.ConnectionId);
        }
        else
        {
            _logger.LogDebug(
                "TmdbProgressHub: client disconnected cleanly — ConnectionId={ConnectionId}",
                Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
