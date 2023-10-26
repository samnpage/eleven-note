using ElevenNote.Services.User;
using Microsoft.AspNetCore.Mvc;

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
}