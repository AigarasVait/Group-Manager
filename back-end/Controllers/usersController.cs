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

    public UsersController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Validates username/password combo.
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateUser([FromBody] User user)
    {
        // storing passwords in plain text is not safe, but for simplicity i did not hash them
        var existingUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);

        if (existingUser == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var userDto = _mapper.Map<UserDto>(existingUser);
        return Ok(userDto);
    }

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var isUsernameTaken = await _db.Users.AnyAsync(u => u.Username == user.Username);
        if (isUsernameTaken)
        {
            return BadRequest("Username is already taken.");
        }

        // Default the name to the username if not provided
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            user.Name = user.Username;
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var userDto = _mapper.Map<UserDto>(user);
        return Created("", userDto);
    }
}
