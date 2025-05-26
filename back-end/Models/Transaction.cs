using System.Text.Json.Serialization;
using back_end.Models;

/*
    This class represents a transaction in the system.
    Each transaction has an ID, a description, a date, an amount, a payer (user), and a group.
    The Transaction class is used to manage the financial transactions within a group.
    The Payer property is a reference to the User class, which represents the user who made the transaction.
*/

public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }

    public SplitType SType { get; set; }
    public decimal[] SplitValues { get; set; } = null!;

    public int PayerId { get; set; }
    [JsonIgnore]
    public User Payer { get; set; } = null!;

    public int GroupId { get; set; }
    [JsonIgnore]
    public Group Group { get; set; } = null!;
}

public enum SplitType
{
    Equal = 0,
    Dynamic = 1,
    Percentage = 2
}

public class TransactionCreateDto
{
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public SplitType SType { get; set; }
    public decimal[] SplitValues { get; set; } = null!;
    public int PayerId { get; set; }
    public int GroupId { get; set; }
}

public class TransactionDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
      public DateTime Date { get; set; }
    public int PayerId { get; set; }
    public Group Group { get; set; } = null!;
}