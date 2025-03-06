using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Managers;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "User")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("GET")]
    [Authorize(Policy = "Admin")]
    public ActionResult<List<User>> Get()
    {
        var users = _userService.GetAll();
        Console.WriteLine(users);
        return Ok(users); // גישה לכל המשתמשים
    }

    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);
        Console.WriteLine("user:" + user);
        if (user == null)
        { return NotFound("user not found"); }
        return Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public ActionResult Insert(User newUser)
    {
        var users = _userService.GetAll();
        newUser.Id = users.Count != 0 ? users.Max(u => u.Id) + 1 : 1;
        _userService.GetAll().Add(newUser);
        _userService.SaveJewelrys(users);
        return CreatedAtAction(nameof(Insert), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Admin")]
    public ActionResult Update(int id, User newUser)
    {
        var users = _userService.GetAll();
        var oldUser = users.FirstOrDefault(p => p.Id == id);
        if (oldUser == null)
            return NotFound("User not found");
        if (newUser.Id != oldUser.Id)
            return BadRequest("id mismatch");
        oldUser.UserName = newUser.UserName;
        oldUser.Password = newUser.Password;
        _userService.SaveJewelrys(users);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public ActionResult Delete(int id)
    {
        var users = _userService.GetAll();
        var dUser = users.FirstOrDefault(p => p.Id == id);
        if (dUser == null)
            return NotFound("invalid id");
        users.Remove(dUser);
        _userService.SaveJewelrys(users);
        return NoContent();
    }
}
