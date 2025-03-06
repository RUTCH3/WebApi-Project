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
        return Ok(_jewelryService.GetAll());
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
