using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolarizedJam.Database;

namespace PolarizedJam.Controllers;

[ApiController]
[Route("[controller]")]
public class JamController : ControllerBase
{
    private readonly DatabaseContext _db;

    public JamController(DatabaseContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create()
    {
        var jam = new Jam()
        {
            Id = Guid.NewGuid(),
        };
        _db.Add(jam);
        await _db.SaveChangesAsync();
        return Ok(jam.Id);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Jam>> Get([FromRoute] Guid id)
    {
        var jam = await _db.Jams.FirstOrDefaultAsync(x => x.Id == id);
        return Ok(jam);
    }
}
