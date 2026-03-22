namespace Data.Dtos;

public record JobPostUrlRequestDto(
    List<string> Links
);

public record RoadmapResponseDto(
    int Id,
    string Roadmap
);
