using ECommerce.Core.GenralResponse;
using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;


public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimiter _rateLimiter;

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
        _rateLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 2,
            Window = TimeSpan.FromSeconds(10),
            AutoReplenishment = true
        });
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        using var lease = await _rateLimiter.AcquireAsync(permitCount: 1);

        if (lease.IsAcquired)
        {
            await _next(context);
        }
        else
        {
            context.Response.StatusCode = 429;
            context.Response.ContentType = "application/json";

            var response = Response<object>.Fail("Too many requests. Please try again later.");

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    }
}