namespace Data.Dtos;

public record JobPostUrlRequestDto(
    List<string> Links
);

public record RoadmapResponseDto(
    string Roadmap
);
