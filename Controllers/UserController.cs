using Data.Entities;
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

    [HttpGet("all")]
    public async Task<IActionResult> GetUsers()
     => new JsonResult(await _userService.GetUsers());

    [HttpPost("add")]
    public async Task<IActionResult> AddUser([FromBody] User user)
     => new JsonResult(await _userService.AddUser(user));

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
     => new JsonResult(await _userService.UpdateUser(id, user));

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
     => new JsonResult(await _userService.DeleteUser(id));

}