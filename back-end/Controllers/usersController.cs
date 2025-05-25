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

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var duplicate = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (duplicate == null)
        {
            _db.Users.Add(user);
        await _db.SaveChangesAsync();
        var userDto = _mapper.Map<UserDto>(user);
        return Created("", userDto);
        }
        else
        {
            return BadRequest("Username taken.");
        }
    }
}