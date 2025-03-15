using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Services;

namespace SothbeysKillerApi.Controllers;

public record UserCreateRequest(string Name, string Email, string Password);
public record UserSigninRequest(string Email, string Password);
public record UserSigninResponse(Guid Id, string Name, string Email);

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
};


[ApiController]
[Route("api/v1/[controller]/[action]")]
public class UserController : ControllerBase
{

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public IActionResult Signup(UserCreateRequest request)
    {
        try
        {
           _userService.SignupUser(request);
            return NoContent();//status code 204
        }
        catch(ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult Signin(UserSigninRequest request)
    {
        try
        {
            var response = _userService.SigninUser(request);
            return Ok(response);//status code 200
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return Unauthorized();//status code 401
        }
        
    }

}


