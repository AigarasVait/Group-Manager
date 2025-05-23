namespace back_end.Models;

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
}
