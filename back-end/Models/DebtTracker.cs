using back_end.Models;
using System.Text.Json.Serialization;

public class DebtTracker
{
    public int Id { get; set; }

    // User who owes money
    public int FromUserId { get; set; }
    public User FromUser { get; set; } = null!;

    // User who is owed money
    public int ToUserId { get; set; }
    public User ToUser { get; set; } = null!;

    // Amount owed (positive decimal)
    public decimal Amount { get; set; }

    // Reference to the group related to this debt
    // Ignored in JSON serialization to avoid circular references or excess data
    [JsonIgnore]
    public int GroupId { get; set; }

    [JsonIgnore]
    public Group Group { get; set; } = null!;
}
