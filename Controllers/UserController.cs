using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
     => new JsonResult(await _userService.Login(request.Email, request.Password));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
     => new JsonResult(await _userService.Register(request.Email, request.Password));

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

}