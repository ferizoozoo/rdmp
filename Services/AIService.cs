using System.Text;
using System.Text.Json;
using Data.Entities;

namespace Services;

public interface IAIService
{
    Task<RoadmapResponseDto> GenerateRoadmapAsync(JobPostUrlRequestDto jobPostUrl);
}

public class AIService : IAIService
{
    private string GEMINI_API_KEY = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
    private readonly HttpClient client;
    private readonly ICrawlerService _crawlerService;
    private const string GEMINI_API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent";

    public AIService(HttpClient httpClient, ICrawlerService crawlerService)
    {
        client = httpClient;
        _crawlerService = crawlerService;
    }

    public async Task<RoadmapResponseDto> GenerateRoadmapAsync(JobPostUrlRequestDto jobPostUrl)
    {
        var jobDescription = await _crawlerService.CrawlJobPostingAsync(jobPostUrl.Url);
        var prompt = $"Generate a learning roadmap based on the description of the job posting or about the work from the given job description (don't include any other text): {jobDescription}";
        Console.WriteLine($"Generated prompt: {prompt}");

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        string jsonPayload = JsonSerializer.Serialize(requestBody);

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, GEMINI_API_URL)
        {
            Content = content
        };

        request.Headers.Add("x-goog-api-key", GEMINI_API_KEY);

        try
        {
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseBody);
            var roadmap = responseJson.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
            return new RoadmapResponseDto(roadmap);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return new RoadmapResponseDto("Failed to generate roadmap.");
        }
    }
}