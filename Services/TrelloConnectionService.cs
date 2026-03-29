using Data.Database;
using Data.Dtos;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public interface ITrelloConnectionService
{
    Task<bool> HasConnection(int userId);
    Task<TrelloConnectionStatusResponse> GetStatus(int userId);
    Task<TrelloConnectionStatusResponse> Connect(int userId, string token);
    Task<bool> Disconnect(int userId);
    Task<TrelloConnection> GetRequiredConnection(int userId);
}

public class TrelloConnectionService : ITrelloConnectionService
{
    private readonly RdmpContext _context;
    private readonly ITrelloService _trelloService;

    public TrelloConnectionService(
        RdmpContext context,
        ITrelloService trelloService)
    {
        _context = context;
        _trelloService = trelloService;
    }

    public async Task<bool> HasConnection(int userId)
        => await _context.TrelloConnections
            .AsNoTracking()
            .AnyAsync(c => c.UserId == userId);

    public async Task<TrelloConnectionStatusResponse> GetStatus(int userId)
    {
        var connection = await _context.TrelloConnections
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId);

        return new TrelloConnectionStatusResponse
        {
            IsConnected = connection is not null,
            Username = connection?.Username,
            MemberId = connection?.MemberId,
            ConnectedAt = connection?.ConnectedAt
        };
    }

    public async Task<TrelloConnectionStatusResponse> Connect(int userId, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("A Trello token is required.");
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new KeyNotFoundException("User not found.");
        }

        var profile = await _trelloService.GetMemberProfile(token.Trim());
        var existingConnection = await _context.TrelloConnections
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (existingConnection is null)
        {
            existingConnection = new TrelloConnection
            {
                UserId = userId
            };

            _context.TrelloConnections.Add(existingConnection);
        }

        existingConnection.Token = token.Trim();
        existingConnection.MemberId = profile.Id;
        existingConnection.Username = profile.Username;
        existingConnection.ConnectedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new TrelloConnectionStatusResponse
        {
            IsConnected = true,
            Username = existingConnection.Username,
            MemberId = existingConnection.MemberId,
            ConnectedAt = existingConnection.ConnectedAt
        };
    }

    public async Task<bool> Disconnect(int userId)
    {
        var connection = await _context.TrelloConnections
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (connection is null)
        {
            return false;
        }

        _context.TrelloConnections.Remove(connection);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TrelloConnection> GetRequiredConnection(int userId)
        => await _context.TrelloConnections
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId)
           ?? throw new InvalidOperationException("User has not connected a Trello account.");
}
