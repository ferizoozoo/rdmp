using Data.Dtos;
using Data.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpPost("roadmap")]
    public async Task<IActionResult> SendPrompt([FromBody] JobPostUrlRequestDto request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User id claim is missing or invalid.");
        }

        var roadmap = await aiService.GenerateRoadmapAsync(request, userId);
        return Ok(roadmap);
    }
}
