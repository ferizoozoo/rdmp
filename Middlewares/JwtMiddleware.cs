using System.Security.Claims;
using Services;

namespace Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        var token = context.Request.Headers.Authorization
            .FirstOrDefault()?.Split(" ").Last();

        if (token != null && jwtService.ValidateAccessToken(token, out var userId))
        {
            context.Items["UserId"] = userId;

            // Optionally hydrate HttpContext.User so [Authorize] still works
            var claims = new[] { new Claim("userId", userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "Jwt");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}