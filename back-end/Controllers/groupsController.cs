using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Models;
using AutoMapper;
using back_end.Helper;

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

    /// <summary>
    /// Returns all groups that a user is a member of, along with their balances.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetGroups([FromQuery] int userId)
    {
        if (userId <= 0)
            return BadRequest($"Invalid user ID: {userId}");

        // Grabs all groups this user is in, including members and debt info
        var groups = await _db.Groups
            .Include(g => g.Members)
            .Include(g => g.DebtTrackers)
            .Where(g => g.Members.Any(m => m.Id == userId))
            .ToListAsync();


        var groupsDto = _mapper.Map<List<GroupSimpleDto>>(groups);

        // Add balance data for each group, for this user
        for (int i = 0; i < groupsDto.Count; i++)
        {
            groupsDto[i].Balance = BalanceCalculator.Total(groups[i], userId);
        }

        return Ok(groupsDto);
    }

    /// <summary>
    /// Returns a single group by ID, including its members and transactions.
    /// </summary>
    [HttpGet("single")]
    public async Task<IActionResult> GetGroup([FromQuery] int userId, int groupId)
    {
        if (userId <= 0 || groupId <= 0)
            return BadRequest($"Invalid user ID: {userId} or group ID: {groupId}");

        // Only get group if the user is a member
        var group = await _db.Groups
            .Include(g => g.Members)
            .Include(g => g.Transactions)
            .Include(g => g.DebtTrackers)
            .FirstOrDefaultAsync(g =>
                g.Id == groupId && g.Members.Any(m => m.Id == userId));

        if (group == null)
            return NotFound($"Group with ID {groupId} not found for user {userId}");

        var groupDto = _mapper.Map<GroupDto>(group);

        // For each member, calculate their balance from current user's perspective
        foreach (var member in groupDto.Members)
        {
            member.Balance = BalanceCalculator.Personal(group, userId, member.Id);
        }

        return Ok(groupDto);
    }

    /// <summary>
    /// Creates a new group with the specified name and creator.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] GroupPostDto groupPost)
    {
        var group = new Group
        {
            Name = groupPost.Name
        };

        var creator = await _db.Users.FindAsync(groupPost.CreatorId);
        if (creator == null)
            return BadRequest("Creator user not found.");

        // New group starts with only the creator as a member
        group.Members = [creator];

        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        var groupDto = _mapper.Map<GroupDto>(group);
        return Created("", groupDto);
    }

    /// <summary>
    /// Patches a group to add or remove members, or mark debts as paid.
    /// </summary>
    [HttpPatch("{groupId}")]
    public async Task<IActionResult> PatchGroup(int groupId, [FromBody] GroupPatchDto patch)
    {
        var group = await _db.Groups
            .Include(g => g.Members)
            .Include(g => g.DebtTrackers)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            return NotFound();

        // === Add member ===
        if (patch.AddMemberUserId.HasValue)
        {
            var user = await _db.Users.FindAsync(patch.AddMemberUserId.Value);
            if (user == null)
                return NotFound("User not found");

            if (!group.Members.Any(u => u.Id == user.Id))
                group.Members.Add(user);
        }

        // === Remove member ===
        else if (patch.RemoveMemberUserId.HasValue)
        {
            var user = group.Members.FirstOrDefault(u => u.Id == patch.RemoveMemberUserId.Value);
            if (user == null)
                return Ok(); 

            // Block removal if the user has any debts (owed or owing)
            bool hasDebt = group.DebtTrackers.Any(dt =>
                (dt.FromUserId == user.Id || dt.ToUserId == user.Id) &&
                dt.Amount != 0
            );

            if (hasDebt)
                return BadRequest("Cannot remove user with outstanding debts.");

            // Clean up any debt records related to this user
            group.DebtTrackers.RemoveAll(dt =>
                dt.FromUserId == user.Id || dt.ToUserId == user.Id);

            group.Members.Remove(user);
        }

        // === Mark a debt as paid ===
        else if (patch.PaidMemberUserId.HasValue && patch.FromMemberUserId.HasValue)
        {
            var tracker = group.DebtTrackers.FirstOrDefault(dt =>
                (dt.FromUserId == patch.PaidMemberUserId.Value ||
                 dt.ToUserId == patch.PaidMemberUserId.Value));

            if (tracker == null)
                return BadRequest("No valid debt found.");


            tracker.Amount = 0;
        }


        else
        {
            return BadRequest("No valid patch operation provided.");
        }

        await _db.SaveChangesAsync();
        return Ok(group);
    }
}
