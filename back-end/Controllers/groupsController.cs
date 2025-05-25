using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Models;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GroupsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }


    [HttpGet]
public async Task<IActionResult> GetGroups([FromQuery] int userId)
{
    if (userId <= 0)
    {
        return BadRequest($"Invalid user ID: {userId}");
    }

    var groups = await _db.Groups
        .Include(g => g.Members)
        .Where(g => g.Members.Any(m => m.Id == userId))
        .ToListAsync();
    var groupsDto = _mapper.Map<List<GroupDto>>(groups);
    return Ok(groupsDto);
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
            group.Members = [creator];
        }
        else
        {
            return BadRequest("Creator user not found.");
        }

        _db.Groups.Add(group);
        await _db.SaveChangesAsync();
        var groupDto = _mapper.Map<GroupDto>(group);
        return Created("", groupDto);
    }
}