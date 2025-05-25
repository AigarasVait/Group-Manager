namespace back_end.Models;
using System.Text.Json.Serialization;

/*
    This class represents a user in the system.
    Each user has an ID, a name, a username, a very secure password and a list of groups they belong to.
    
*/
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    [JsonIgnore]
    public List<Group> Groups { get; set; } = [];
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}