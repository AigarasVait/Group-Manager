namespace back_end.Models;
using System.Text.Json.Serialization;

/*
    Represents a group containing members and transactions.
    Each group has an Id, Name, list of members (Users), and transactions.
    DebtTrackers keep track of debts inside the group, but are ignored during JSON serialization.
*/
public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // Members of the group
    public List<User> Members { get; set; } = new List<User>();

    // Transactions related to this group
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    // Tracks debts within the group; ignored in JSON responses to avoid circular references
    [JsonIgnore]
    public List<DebtTracker> DebtTrackers { get; set; } = new List<DebtTracker>();
}

/*
    DTO used when creating a new group.
    Contains the group's Name and the Creator's UserId.
*/
public class GroupPostDto
{
    public string Name { get; set; } = "";
    public int CreatorId { get; set; }
}

/*
    Simple DTO for listing groups.
    Includes group Id, Name, and the balance for the current user.
*/
public class GroupSimpleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Balance { get; set; } = 0;
}

/*
    Detailed DTO representing a group with members and transactions.
*/
public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<UserDto> Members { get; set; } = new List<UserDto>();
    public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
}

/*
    DTO for partial updates to a group.
    Allows adding/removing members or marking debts as paid.
*/
public class GroupPatchDto
{
    public int? AddMemberUserId { get; set; }
    public int? RemoveMemberUserId { get; set; }
    public int? PaidMemberUserId { get; set; }
    public int? FromMemberUserId { get; set; }
}
