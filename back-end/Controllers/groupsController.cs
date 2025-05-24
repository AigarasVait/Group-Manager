using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Models;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _db;

    public GroupsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/groups
    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _db.Groups.ToListAsync();
        return Ok(groups);
    }

    // POST: api/groups
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] Group group)
    {
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGroups), new { id = group.Id }, group);
    }
}