using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SothbeysKillerApi.Controllers;

public record UserCreateRequest(string Name, string Email, string Password);
public record UserSigninRequest(string Email, string Password);
public record UserSigninRespons(Guid Id, string Name, string Email);

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
};


[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{

    private static List<User> _users = [];

    [HttpPost]
    [Route("[action]")]
    public IActionResult Signup(UserCreateRequest request)
    {

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 3 || request.Name.Length > 60)
        {
            return BadRequest();//status code 400
        }

        if (string.IsNullOrEmpty(request.Email) || _users.Any(a => a.Email == request.Email))
        {
            return BadRequest();
        }

        int atIndex = request.Email.IndexOf('@');

        if (atIndex <= 0 || atIndex != request.Email.LastIndexOf('@'))
        {
            return BadRequest();
        }

        string local = request.Email[..atIndex];
        string domain = request.Email[(atIndex + 1)..];

        if (domain.Length < 3 || !domain.Contains('.'))
        {
            return BadRequest();
        }

        string topLevelDomain = (domain.Split('.'))[^1];

        if (topLevelDomain.Length < 2 || topLevelDomain.Length > 6)
        {
            return BadRequest();
        }

        if (!(local.All(l => char.IsLetterOrDigit(l) || l is '.' or '-') && domain.All(l => char.IsLetterOrDigit(l) || l is '.' or '-')))
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return BadRequest();
        }

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };

        _users.Add(user);

        return NoContent();//status code 204
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Signin(UserSigninRequest request)
    {
        var user = _users.FirstOrDefault(a => a.Email == request.Email);

        if (user is null)
        {
            return NotFound();//status code 404
        }
        if (!(user.Password.Equals(request.Password)))
        {
            return Unauthorized();//status code 401
        }
        var response = new UserSigninRespons(user.Id, user.Name, user.Email);
        return Ok(response);//status code 200
    }

}


