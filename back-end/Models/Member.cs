namespace back_end.Models;

public class Member
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public List<Group> Groups { get; set; } = [];
}