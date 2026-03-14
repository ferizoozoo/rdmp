using Data.Entities;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("all")]
    public async Task<IActionResult> GetRoadmaps()
     => new JsonResult(await _roadmapService.GetRoadmaps());

    [HttpPost("add")]
    public async Task<IActionResult> AddRoadmap([FromBody] Roadmap roadmap)
     => new JsonResult(await _roadmapService.AddRoadmap(roadmap));

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRoadmap(int id, [FromBody] Roadmap roadmap)
     => new JsonResult(await _roadmapService.UpdateRoadmap(id, roadmap));

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRoadmap(int id)
     => new JsonResult(await _roadmapService.DeleteRoadmap(id));

}