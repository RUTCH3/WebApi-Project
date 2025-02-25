using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Managers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize(Policy = "Admin")]
        public IEnumerable<User> GetAllUsers()
        {
            return _userService.GetAll() ?? [];
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult<string> Login([FromBody] User User)
        {
            var users = _userService.GetAll()?.FirstOrDefault(user => user.Id == User.Id && user.Password == User.Password);
            if (users == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new("type", "User"),
                new("type", "Admin"),

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
                new("type", "Agent"),
                new("ClearanceLevel", user.UserName ?? "unknown user"),
            };

            var token = TokenService.GetToken(claims);

            return new OkObjectResult(TokenService.WriteToken(token));
        }
    }



}
