namespace Data.Dtos;

public class TrelloConnectRequest
{
    public string Token { get; set; } = string.Empty;
}

public class TrelloConnectionStatusResponse
{
    public bool IsConnected { get; set; }
    public string? Username { get; set; }
    public string? MemberId { get; set; }
    public DateTime? ConnectedAt { get; set; }
}

public class TrelloExportResponse
{
    public bool RequiresAuthorization { get; set; }
    public string BoardId { get; set; } = string.Empty;
    public string BoardName { get; set; } = string.Empty;
    public string BoardUrl { get; set; } = string.Empty;
}

public class TrelloMemberProfile
{
    public string Id { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
}

public class TrelloBoardResult
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}

public class TrelloListResult
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}

public class TrelloCardResult
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}
