using System.Text.Json;
using Data.Dtos;

namespace Helpers;

public class RoadmapDeserializer
{
    public static RoadmapDocument? DeserializeRoadmap(string content)
    {
        try
        {
            return JsonSerializer.Deserialize<RoadmapDocument>(content);
        }
        catch (JsonException)
        {
            // Handle JSON deserialization errors if necessary
            return null;
        }
    }
}