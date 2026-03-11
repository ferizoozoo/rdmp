using System.Text;
using System.Text.Json;
using Data.Dtos;

namespace Services;

public interface IAIService
{
    Task<RoadmapResponseDto> GenerateRoadmapAsync(JobPostUrlRequestDto jobPostUrl);
}

public class AIService : IAIService
{
    private string GEMINI_API_KEY = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
    private string GEMINI_API_URL = Environment.GetEnvironmentVariable("GEMINI_API_URL") ?? string.Empty;
    private readonly HttpClient client;
    private readonly ICrawlerService _crawlerService;

    public AIService(HttpClient httpClient, ICrawlerService crawlerService)
    {
        client = httpClient;
        _crawlerService = crawlerService;
    }

    public async Task<RoadmapResponseDto> GenerateRoadmapAsync(JobPostUrlRequestDto jobPostUrl)
    {
        var jobDescription = await _crawlerService.CrawlJobPostingAsync(jobPostUrl.Link);
        var prompt = $@"Generate a learning roadmap based on the description of
         the job posting or about the work from the given job description (don't
         include any other text): {jobDescription}
         The roadmap should be in a format that can be easily parsed and displayed in a user interface.
         The roadmap should include the following sections:
         1. Skills: A list of skills required for the job, along with a brief description of each skill and resources for learning them.
         2. Projects: A list of project ideas that can help someone build a portfolio relevant to the job, along with a brief description of each project and resources for learning how to build them.
         3. Timeline: A suggested timeline for learning the skills and building the projects, based on the typical experience level required for the job.
         The roadmap should be concise and focused on the most important skills and projects for the job, and should not include any extraneous information. The roadmap should be formatted in a way
         that is easy to read and understand, with clear headings and bullet points for each section. The roadmap should be tailored
         remove the stars";


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