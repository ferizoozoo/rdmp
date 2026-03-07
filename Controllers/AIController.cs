using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Endpoints;

[ApiController]
[Route("[controller]")]
public class AIController : ControllerBase
{
    private readonly IAIService aiService;

    public AIController(IAIService aiService)
    {
        this.aiService = aiService;
    }

    [HttpPost("roadmap")]
    public async Task<IActionResult> SendPrompt([FromBody] JobPostUrlRequestDto request)
    {
        var roadmap = await aiService.GenerateRoadmapAsync(request);
        return Ok(roadmap);
    }
}