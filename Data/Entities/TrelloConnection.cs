using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Data.Entities;

public class TrelloConnection
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public User? User { get; set; }
}
