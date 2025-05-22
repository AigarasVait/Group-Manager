namespace back_end.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Member> Members { get; set; } = [];
}
