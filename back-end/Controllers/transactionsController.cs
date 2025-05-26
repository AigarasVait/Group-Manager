using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Models;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public TransactionsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDto transaction )
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