using back_end.Models;
using System.Text.Json.Serialization;
public class DebtTracker
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public User FromUser { get; set; } = null!;
    public int ToUserId { get; set; }
    public User ToUser { get; set; } = null!;
    public decimal Amount { get; set; }
    [JsonIgnore]
    public int GroupId { get; set; }
    [JsonIgnore]
    public Group Group { get; set; } = null!;
}