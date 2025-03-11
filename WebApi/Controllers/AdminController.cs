using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Managers;
using WebApi.Models;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Policy = "Admin")]
        public IEnumerable<User> GetAllUsers()
        {
            return _userService.GetAll();
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult<string> Login([FromBody] User User1)
        {
            Console.WriteLine($"Received login request: {User1.UserName}, {User1.Password} ,{User1.Type}");

            User? users = _userService.GetAll()?.FirstOrDefault(user => user.UserName == User1.UserName && user.Password == User1.Password);
            Console.WriteLine("users: " + users.Id + " " + users.UserName + " " + users.Type + " " + users.Password);
            if (users == null)
            // if (User1.UserName != "Ruti" || User1.Password != "rutich33013")
            {
                return Unauthorized("משתמש לא נמצא");
            }

            var claims = new List<Claim>
            {
                new("type", "User"),
                new("type", "Admin")
            };
            var token = TokenService.GetToken(claims);
            return new OkObjectResult(TokenService.WriteToken(token));
        }


        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "Admin")]
        public IActionResult GenerateBadge([FromBody] User user)
        {
            var claims = new List<Claim>
            {
                new("type", "User"),
                new("ClearanceLevel", user.UserName ?? "unknown user"),
            };

            var token = TokenService.GetToken(claims);

            return new OkObjectResult(TokenService.WriteToken(token));
        }
    }

}
