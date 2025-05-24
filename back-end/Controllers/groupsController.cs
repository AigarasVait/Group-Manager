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


    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _db.Groups.ToListAsync();
        return Ok(groups);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] GroupPost groupPost)
    {
        var group = new Group
        {
            Name = groupPost.Name
        };

        var creator = await _db.Users.FindAsync(groupPost.CreatorId);
        if (creator != null)
        {
            group.Members = new List<User> { creator };
        }
        else
        {
            return BadRequest("Creator user not found.");
        }

        _db.Groups.Add(group);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGroups), new { id = group.Id }, group);
    }
}