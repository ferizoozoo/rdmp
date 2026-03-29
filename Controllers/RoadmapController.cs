using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Services;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class RoadmapController : ControllerBase
{
    private readonly IRoadmapService _roadmapService;

    public RoadmapController(IRoadmapService roadmapService)
    {
        _roadmapService = roadmapService;
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetRoadmaps()
     => new JsonResult(await _roadmapService.GetRoadmaps());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoadmap(int id)
     => new JsonResult(await _roadmapService.GetRoadmap(id));

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetRoadmapByUserId(int userId)
     => new JsonResult(await _roadmapService.GetRoadmapByUserId(userId));

    [Authorize]
    [HttpGet("user/{userId}/all")]
    public async Task<IActionResult> GetRoadmapsByUserId(int userId)
         => new JsonResult(await _roadmapService.GetRoadmapsByUserId(userId));

    [Authorize]
    [HttpPost("export/trello/{roadmapId}")]
    public async Task<IActionResult> ExportToTrello(int roadmapId)
        => new JsonResult(await _roadmapService.ExportToTrello(roadmapId, GetCurrentUserId()));


    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddRoadmap([FromBody] Roadmap roadmap)
     => new JsonResult(await _roadmapService.AddRoadmap(roadmap));

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRoadmap(int id, [FromBody] Roadmap roadmap)
     => new JsonResult(await _roadmapService.UpdateRoadmap(id, roadmap));

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRoadmap(int id)
     => new JsonResult(await _roadmapService.DeleteRoadmap(id));

    private int GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");
        if (!int.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("Authenticated user id is missing.");
        }

        return parsedUserId;
    }
}
