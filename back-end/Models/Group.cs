namespace back_end.Models;
using System.Text.Json.Serialization;

/*
    This class represents a group in the system.
    Each group has an ID, a name, a list of members (users), and a list of transactions.
    
    The Group class is used to manage the members and transactions within a group.
*/

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public List<User> Members { get; set; } = [];

    public List<Transaction> Transactions { get; set; } = [];

    [JsonIgnore]
    public List<DebtTracker> DebtTrackers { get; set; } = [];
}

/*
    This class represents a post that can be made in a group.
    It contains the name of the post and the ID of the user who created it.
    
    The GroupPost class is used to represent posts made by users.
*/
public class GroupSimpleDto
{
    public string Name { get; set; } = "";
    public int CreatorId { get; set; }
    public decimal Balance { get; set; } = 0;
}

public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<UserDto> Members { get; set; } = [];
    public List<TransactionDto> Transactions { get; set; } = [];
}

public class GroupPatchDto
{
    public int? AddMemberUserId { get; set; }
    public int? RemoveMemberUserId { get; set; }
}
