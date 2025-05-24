using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Models;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UsersController(AppDbContext db,IMapper mapper)
    {
        _mapper = mapper;
        _db = db;
    }


    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _db.Groups.ToListAsync();
        return Ok(groups);
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateUser([FromBody] User user)
    {
        var existingUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);

        if (existingUser != null)
        {
            var userDto = _mapper.Map<UserDto>(existingUser);
            return Ok(userDto);
        }
        else
        {
            return Unauthorized("Invalid username or password.");
        }
    }

    // [HttpPost("create")]
    // public async Task<IActionResult> CreateUser([FromBody] GroupPost groupPost)
    // {
    //     var group = new Group
    //     {
    //         Name = groupPost.Name
    //     };

    //     var creator = await _db.Users.FindAsync(groupPost.CreatorId);
    //     if (creator != null)
    //     {
    //         group.Members = new List<User> { creator };
    //     }
    //     else
    //     {
    //         return BadRequest("Creator user not found.");
    //     }

    //     _db.Groups.Add(group);
    //     await _db.SaveChangesAsync();
    //     return CreatedAtAction(nameof(GetGroups), new { id = group.Id }, group);
    // }
}