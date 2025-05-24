namespace back_end.Models;

/*
    This class represents a user in the system.
    Each user has an ID, a name, a username, and a list of groups they belong to.
    
*/
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public List<Group> Groups { get; set; } = [];
}