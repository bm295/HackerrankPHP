using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new
{
    service = "Stateless HTTP demo",
    note = "Each request is self-contained. Authentication and request context are read from headers every time."
}));

app.MapGet("/whoami", (HttpRequest request) =>
{
    if (!TryParseBearerToken(request, out var claims, out var error))
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new
    {
        subject = claims.FindFirstValue(ClaimTypes.NameIdentifier),
        role = claims.FindFirstValue(ClaimTypes.Role),
        requestId = request.Headers["X-Request-Id"].ToString()
    });
});

app.MapPost("/echo", (HttpRequest request, EchoPayload payload) =>
{
    if (!TryParseBearerToken(request, out var claims, out _))
    {
        return Results.Unauthorized();
    }

    var tenant = request.Headers["X-Tenant-Id"].ToString();
    if (string.IsNullOrWhiteSpace(tenant))
    {
        return Results.BadRequest(new { error = "X-Tenant-Id header is required." });
    }

    return Results.Ok(new
    {
        tenant,
        user = claims.FindFirstValue(ClaimTypes.NameIdentifier),
        payload.Message,
        payload.TimestampUtc,
        handledStatelessly = true
    });
});

app.Run();

static bool TryParseBearerToken(HttpRequest request, out ClaimsPrincipal claims, out string? error)
{
    claims = new ClaimsPrincipal();
    error = null;

    var authHeader = request.Headers.Authorization.ToString();
    if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        error = "Missing Bearer token.";
        return false;
    }

    var token = authHeader["Bearer ".Length..].Trim();

    var parts = token.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (parts.Length != 2)
    {
        error = "Token format must be `userId:role`.";
        return false;
    }

    var identity = new ClaimsIdentity("Bearer");
    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, parts[0]));
    identity.AddClaim(new Claim(ClaimTypes.Role, parts[1]));
    claims = new ClaimsPrincipal(identity);

    return true;
}

internal sealed record EchoPayload(string Message, DateTime TimestampUtc);
