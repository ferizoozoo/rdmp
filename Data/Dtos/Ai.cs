namespace Data.Dtos;

public record JobPostUrlRequestDto(
    List<string>? Links,
    string? Description
);

public record RoadmapResponseDto(
    int Id,
    string Roadmap
);
