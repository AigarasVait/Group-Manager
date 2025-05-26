using System.Text.Json.Serialization;
using back_end.Models;

/*
    Represents a financial transaction within a group.
    Includes details about the amount, payer, and how the cost is split.
*/
public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }

    public SplitType SType { get; set; }
    
    // Split values correspond to members of the group, length should match members count.
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
    Equal = 0,       // Even split among members
    Dynamic = 1,     // Custom amounts per member
    Percentage = 2   // Percent-based split
}

/*
    DTO for creating a new transaction.
*/
public class TransactionCreateDto
{
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public SplitType SType { get; set; }
    public decimal[] SplitValues { get; set; } = null!;
    public int PayerId { get; set; }
    public int GroupId { get; set; }
}

/*
    DTO representing a transaction returned by API.
    Note: Group is included fully here — consider mapping a lighter Group DTO to avoid overfetching.
*/
public class TransactionDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int PayerId { get; set; }

    // Consider changing this to a lighter DTO for safer serialization
    public Group Group { get; set; } = null!;
}
