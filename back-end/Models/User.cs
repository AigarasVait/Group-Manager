namespace back_end.Models;
using System.Text.Json.Serialization;

/*
    Represents a user in the system.
    Each user has an ID, name, username, a password (should be hashed in a real app),
    and a list of groups they belong to.
*/
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    
    // NOTE: For production, store hashed passwords instead of plain text.
    public string Password { get; set; } = "";

    [JsonIgnore]
    public List<Group> Groups { get; set; } = [];
}

/*
    DTO used to expose limited user info, including the user's balance in a group context.
*/
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Balance { get; set; } = 0;
}
