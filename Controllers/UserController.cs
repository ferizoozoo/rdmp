using Data.Dtos;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Services;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITrelloConnectionService _trelloConnectionService;

    public UserController(
        IUserService userService,
        ITrelloConnectionService trelloConnectionService)
    {
        _userService = userService;
        _trelloConnectionService = trelloConnectionService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
     => new JsonResult(await _userService.Login(request));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
     => new JsonResult(await _userService.Register(request));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
         => new JsonResult(await _userService.Refresh(request));

    [Authorize]
    [HttpPost("trello/connect")]
    public async Task<IActionResult> ConnectTrello([FromBody] TrelloConnectRequest request)
    {
        try
        {
            return new JsonResult(await _trelloConnectionService.Connect(GetCurrentUserId(), request.Token));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [Authorize]
    [HttpGet("trello/status")]
    public async Task<IActionResult> GetTrelloStatus()
        => new JsonResult(await _trelloConnectionService.GetStatus(GetCurrentUserId()));

    [Authorize]
    [HttpDelete("trello/disconnect")]
    public async Task<IActionResult> DisconnectTrello()
        => new JsonResult(await _trelloConnectionService.Disconnect(GetCurrentUserId()));

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetUsers()
     => new JsonResult(await _userService.GetUsers());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
     => new JsonResult(await _userService.GetById(id));

    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddUser([FromBody] User user)
     => new JsonResult(await _userService.AddUser(user));

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
     => new JsonResult(await _userService.UpdateUser(id, user));

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
     => new JsonResult(await _userService.DeleteUser(id));

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
