namespace Data.Dtos;

public record RoadmapCardInput(string Name, string Description);

public class RoadmapDocument
{
    public List<RoadmapSkill> Skills { get; init; } = [];
    public List<RoadmapProject> Projects { get; init; } = [];
    public List<RoadmapTimeline> Timeline { get; init; } = [];
}

public class RoadmapSkill
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> Resources { get; init; } = [];
}

public class RoadmapProject
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> Resources { get; init; } = [];
}

public class RoadmapTimeline
{
    public int Month { get; init; }
    public List<string> SkillsToLearn { get; init; } = [];
    public List<string> ProjectsToBuild { get; init; } = [];
}