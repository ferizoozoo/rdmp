using System.Text.Json;
using Data.Dtos;

namespace Helpers;

public class RoadmapDeserializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static RoadmapDocument? DeserializeRoadmap(string content)
    {
        try
        {
            return JsonSerializer.Deserialize<RoadmapDocument>(content, JsonOptions);
        }
        catch (JsonException)
        {
            // Handle JSON deserialization errors if necessary
            return null;
        }
    }
}
