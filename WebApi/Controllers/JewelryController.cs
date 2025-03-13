using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "User")]
public class JewelryController : ControllerBase
{
    private static List<Jewelry> list;
    private readonly IJewelryService _jewelryService;
    public JewelryController(IJewelryService jewelryService)
    {
        _jewelryService = jewelryService;
    }


    [HttpGet]
    [Route("[action]")]
    public ActionResult<List<Jewelry>> Get()
    {
        try
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                System.Console.WriteLine("User is NOT authenticated.");
                return Unauthorized(new { message = "User is not authenticated." });
            }

            System.Console.WriteLine("User authenticated: " + User.Identity.Name);

            var jewelries = _jewelryService.GetAll();
            return Ok(new
            {
                message = "User is authenticated!",
                User.Identity.Name,
                Jewelries = jewelries
            });
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Exception in Get(): " + ex.Message);
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
        // var user = User.Identity;
        // System.Console.WriteLine("current user: " + user.ToString());
        // if (user == null || !user.IsAuthenticated)
        // {
        //     System.Console.WriteLine("user auth: " + user.IsAuthenticated);
        //     return Unauthorized(new { message = "User is not authenticated." });
        // }
        // System.Console.WriteLine("user auth: " + user.IsAuthenticated);
        // return Ok(new { message = "User is authenticated!", User.Identity.Name, Jewelries = _jewelryService.GetAll() });
    }

    [HttpGet]
    [Route("[action]/{id}")]
    public ActionResult<Jewelry> Get(int id)
    {
        var jewelry = list.FirstOrDefault(p => p.Id == id);
        if (jewelry == null)
            return BadRequest("invalid id");
        return jewelry;
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Policy = "Admin")]
    public ActionResult Insert(Jewelry newJewelry)
    {
        var maxId = list.Max(p => p.Id);
        newJewelry.Id = maxId + 1;
        list.Add(newJewelry);
        _jewelryService.SaveJewelrys(list);
        return CreatedAtAction(nameof(Insert), new { id = newJewelry.Id }, newJewelry);
    }

    [HttpPut]
    [Route("[action]/{id}")]
    [Authorize(Policy = "Admin")]
    public ActionResult Update(int id, Jewelry newJewelry)
    {
        var oldJewelry = list.FirstOrDefault(p => p.Id == id);
        if (oldJewelry == null)
            return BadRequest("invalid id");
        if (newJewelry.Id != oldJewelry.Id)
            return BadRequest("id mismatch");
        oldJewelry.Name = newJewelry.Name;
        oldJewelry.Price = newJewelry.Price;
        _jewelryService.SaveJewelrys(list);
        return Ok(oldJewelry);
    }

    [HttpDelete]
    [Route("[action]/{id}")]
    [Authorize(Policy = "Admin")]
    public ActionResult Delete(int id)
    {
        var dJewelry = list.FirstOrDefault(p => p.Id == id);
        if (dJewelry == null)
            return NotFound("invalid id");
        list.Remove(dJewelry);
        _jewelryService.SaveJewelrys(list);
        return NoContent();
    }
}
