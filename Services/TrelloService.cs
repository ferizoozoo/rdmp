using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Data.Dtos;
using Microsoft.AspNetCore.WebUtilities;

namespace Services;

public interface ITrelloService
{
    string BuildAuthorizationUrl(string returnUrl);
    Task<TrelloMemberProfile> GetMemberProfile(string token);
    Task<TrelloBoardResult> CreateBoard(string token, string boardName);
    Task<TrelloListResult> CreateList(string token, string boardId, string listName);
    Task<TrelloCardResult> CreateCard(string token, string listId, string cardName, string cardDescription);
}

public class TrelloService : ITrelloService
{
    private const string TrelloBaseUrl = "https://api.trello.com/1/";
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TrelloService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(TrelloBaseUrl);
    }

    public string BuildAuthorizationUrl(string returnUrl)
    {
        var apiKey = GetRequiredConfig("TRELLO_API_KEY");
        var appName = _configuration["TRELLO_APP_NAME"] ?? "RDMP";

        var query = new Dictionary<string, string?>
        {
            ["key"] = apiKey,
            ["name"] = appName,
            ["scope"] = "read,write",
            ["expiration"] = "30days",
            ["response_type"] = "token",
            ["callback_method"] = "fragment",
            ["return_url"] = returnUrl
        };

        return QueryHelpers.AddQueryString("https://trello.com/1/authorize", query!);
    }

    public async Task<TrelloMemberProfile> GetMemberProfile(string token)
    {
        var response = await SendAsync(HttpMethod.Get, "members/me", token);
        var payload = await response.Content.ReadFromJsonAsync<TrelloMemberProfile>();

        if (payload is null || string.IsNullOrWhiteSpace(payload.Id))
        {
            throw new InvalidOperationException("Trello did not return a valid member profile.");
        }

        return payload;
    }

    public async Task<TrelloBoardResult> CreateBoard(string token, string boardName)
    {
        var query = new Dictionary<string, string?>
        {
            ["name"] = boardName,
            ["defaultLists"] = "false"
        };

        var response = await SendAsync(HttpMethod.Post, "boards", token, query);
        var payload = await response.Content.ReadFromJsonAsync<TrelloBoardResult>();

        if (payload is null || string.IsNullOrWhiteSpace(payload.Id))
        {
            throw new InvalidOperationException("Trello did not return a board.");
        }

        return payload;
    }

    public async Task<TrelloListResult> CreateList(string token, string boardId, string listName)
    {
        var query = new Dictionary<string, string?>
        {
            ["idBoard"] = boardId,
            ["name"] = listName
        };

        var response = await SendAsync(HttpMethod.Post, "lists", token, query);
        var payload = await response.Content.ReadFromJsonAsync<TrelloListResult>();

        if (payload is null || string.IsNullOrWhiteSpace(payload.Id))
        {
            throw new InvalidOperationException("Trello did not return a list.");
        }

        return payload;
    }

    public async Task<TrelloCardResult> CreateCard(string token, string listId, string cardName, string cardDescription)
    {
        var query = new Dictionary<string, string?>
        {
            ["idList"] = listId,
            ["name"] = cardName,
            ["desc"] = cardDescription
        };

        var response = await SendAsync(HttpMethod.Post, "cards", token, query);
        var payload = await response.Content.ReadFromJsonAsync<TrelloCardResult>();

        if (payload is null || string.IsNullOrWhiteSpace(payload.Id))
        {
            throw new InvalidOperationException("Trello did not return a card.");
        }

        return payload;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string path,
        string token,
        Dictionary<string, string?>? query = null)
    {
        var apiKey = GetRequiredConfig("TRELLO_API_KEY");
        var requestQuery = query ?? [];
        requestQuery["key"] = apiKey;
        requestQuery["token"] = token;

        var requestUri = QueryHelpers.AddQueryString(path, requestQuery!);
        using var request = new HttpRequestMessage(method, requestUri);
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        var errorBody = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("The Trello token is invalid or has been revoked.");
        }

        throw new InvalidOperationException(BuildErrorMessage(response.StatusCode, errorBody));
    }

    private string GetRequiredConfig(string key)
    {
        var value = _configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing required Trello configuration: {key}");
        }

        return value;
    }

    private static string BuildErrorMessage(HttpStatusCode statusCode, string errorBody)
    {
        if (string.IsNullOrWhiteSpace(errorBody))
        {
            return $"Trello request failed with status code {(int)statusCode}.";
        }

        try
        {
            using var document = JsonDocument.Parse(errorBody);
            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty("message", out var messageElement))
            {
                return $"Trello request failed: {messageElement.GetString()}";
            }
        }
        catch (JsonException)
        {
        }

        return $"Trello request failed with status code {(int)statusCode}: {errorBody}";
    }
}
