using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Managers;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "User")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("Get")]
    [Authorize(Policy = "Admin")]
    public ActionResult<List<User>> Get()
    {
        return Ok(_userService.GetUsers()); // גישה לכל המשתמשים
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "User")]
    public ActionResult<User> Get(int id)
    {
        var user = _userService.GetUsers().FirstOrDefault(u => u.Id == id);
            if (user == null)
            {return NotFound("user not found");}
        return Ok(user) ;
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public ActionResult Insert(User newUser)
    {
        var users = _userService.GetUsers();
        newUser.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        _userService.GetUsers().Add(newUser);
        _userService.SaveUsers(users);
        return CreatedAtAction(nameof(Insert), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, User newUser)
    {
        var users = _userService.GetUsers();
        var oldUser = users.FirstOrDefault(p => p.Id == id);
        if (oldUser == null)
            return NotFound("User not found");
        if (newUser.Id != oldUser.Id)
            return BadRequest("id mismatch");
        oldUser.UserName = newUser.UserName;
        oldUser.Password = newUser.Password;
        _userService.SaveUsers(users);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var users = _userService.GetUsers();
        var dUser = users.FirstOrDefault(p => p.Id == id);
        if (dUser == null)
            return NotFound("invalid id");
        users.Remove(dUser);
        _userService.SaveUsers(users);
        return NoContent();
    }
}
