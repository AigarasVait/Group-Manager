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

    [HttpGet("single")]
    public async Task<IActionResult> GetGroups([FromQuery] int userId, int groupId)
    {
        if (userId <= 0 || groupId <= 0)
        {
            return BadRequest($"Invalid user ID: {userId} or group ID: {groupId}");
        }

        var group = await _db.Groups
            .Include(g => g.Members)
            .Include(g => g.Transactions)
            .FirstOrDefaultAsync(g => g.Id == groupId && g.Members.Any(m => m.Id == userId));

        if (group == null)
        {
            return NotFound($"Group with ID {groupId} not found for user with ID {userId}.");
        }

        var groupDto = _mapper.Map<GroupDto>(group);
        return Ok(groupDto);
    }


    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] GroupSimpleDto groupPost)
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
    
    [HttpPatch("{groupId}")]
    public async Task<IActionResult> PatchGroup(int groupId, [FromBody] GroupPatchDto patch)
    {
        var group = await _db.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null) return NotFound();

        if (patch.AddMemberUserId.HasValue)
        {
            var user = await _db.Users.FindAsync(patch.AddMemberUserId.Value);
            if (user == null) return NotFound("User not found");

            if (!group.Members.Any(u => u.Id == user.Id))
            {
                group.Members.Add(user);
            }
        }

        if (patch.RemoveMemberUserId.HasValue)
        {
            var user = group.Members.FirstOrDefault(u => u.Id == patch.RemoveMemberUserId.Value);
            if (user != null)
            {
                group.Members.Remove(user);
            }
        }

        await _db.SaveChangesAsync();
        return Ok(group);
    }
}