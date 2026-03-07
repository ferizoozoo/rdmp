using Data.Entities;

namespace Endpoints;

public static class AIEndpoints
{
    public static void MapAIEndpoints(this IEndpointRouteBuilder routes)
    {
        var api = routes.MapGroup("/ai");
        api.MapPost("/roadmap", (JobPostUrlRequestDto request) => new Services.AIService().GenerateRoadmapAsync(request))
            .WithName("GenerateRoadmap")
            .WithTags("AI");
    }
}