using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ScrumFlix.Services;

/// <summary>
/// Exposes the existing staff session to ASP.NET Core authorization for hubs.
/// A session cookie alone is not an authenticated ClaimsPrincipal.
/// </summary>
public sealed class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ScrumFlixStaffSession";
    private readonly AppDbContext _db;

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        AppDbContext db) : base(options, logger, encoder)
    {
        _db = db;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var session = Context.Session;
        var userId = session.GetInt32(AuthService.SessionUserId);
        var roleId = session.GetInt32(AuthService.SessionRoleId);
        if (userId is null || roleId is not (1 or 2 or 3))
            return AuthenticateResult.NoResult();

        var user = await _db.Users.AsNoTracking()
            .Where(u => u.UserId == userId.Value)
            .Select(u => new { u.UserName, u.RoleId, u.IsActive })
            .FirstOrDefaultAsync(Context.RequestAborted);

        if (user is null || !user.IsActive || user.RoleId != roleId ||
            string.Equals(user.UserName, "web.sales", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.Fail("Staff session is no longer valid.");

        var role = roleId.Value switch
        {
            1 => "Admin",
            2 => "Manager",
            3 => "Employee",
            _ => throw new InvalidOperationException("Unrecognized staff role.")
        };
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role)
            }, SchemeName);
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }
}
