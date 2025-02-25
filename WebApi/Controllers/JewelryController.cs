using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class JewelryController : ControllerBase
{
    private static List<Jewelry> list;
    static JewelryController()
    {
        list =
        [
            new() { Id = 1, Name = "Necklace" ,Price=20.0},
            new() { Id = 2, Name = "Bracelet",Price = 30.4 },
            new() { Id = 3, Name = "Ring",Price = 50.0 },
            new() { Id = 4, Name = "Earrings",Price = 30.4 },
            new() { Id = 5, Name = "Pendant",Price = 30.4 },
            new() { Id = 6, Name = "Brooch",Price = 30.4 },
            new() { Id = 7, Name = "Tiara", Price = 12.4  },
            new() { Id = 8, Name = "Bangle", Price = 24.0  },
            new() { Id = 9, Name = "Crown", Price = 18.9  },
            new() { Id = 10, Name = "Locket", Price = 20  }
        ];
    }

    [HttpGet]
    [Authorize(Policy = "User")]
    public IEnumerable<Jewelry> Get()
    {
        return list;
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "User")]
    public ActionResult<Jewelry> Get(int id)
    {
        var jewelry = list.FirstOrDefault(p => p.Id == id);
        if (jewelry == null)
            return BadRequest("invalid id");
        return jewelry;
    }

    [HttpPost]
    [Authorize(Policy = "User")]
    public ActionResult Insert(Jewelry newJewelry)
    {
        var maxId = list.Max(p => p.Id);
        newJewelry.Id = maxId + 1;
        list.Add(newJewelry);
        return CreatedAtAction(nameof(Insert), new { id = newJewelry.Id }, newJewelry);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "User")]
    public ActionResult Update(int id, Jewelry newJewelry)
    {
        var oldJewelry = list.FirstOrDefault(p => p.Id == id);
        if (oldJewelry == null)
            return BadRequest("invalid id");
        if (newJewelry.Id != oldJewelry.Id)
            return BadRequest("id mismatch");
        oldJewelry.Name = newJewelry.Name;
        oldJewelry.Price = newJewelry.Price;
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "User")]
    public ActionResult Delete(int id)
    {
        var dJewelry = list.FirstOrDefault(p => p.Id == id);
        if (dJewelry == null)
            return NotFound("invalid id");
        list.Remove(dJewelry);
        return NoContent();
    }
}
