using ElevenNote.Models.Responses;
using ElevenNote.Models.User;
using ElevenNote.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ElevenNote.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    // Field
    private readonly IUserService _userService;

    // Constructor
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegister model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var registerResult = await _userService.RegisterUserAsync(model);
        if (registerResult)
        {
            TextResponse response = new("User was registered.");
            return Ok(response);
        }

        return BadRequest(new TextResponse("User could not be registered."));
    }
}