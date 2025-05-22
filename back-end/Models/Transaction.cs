namespace back_end.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int Fk_Group { get; set; }
}