namespace Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var api = routes.MapGroup("/api");
        api.MapGet("/", () => "Hello, World!");
    }
}