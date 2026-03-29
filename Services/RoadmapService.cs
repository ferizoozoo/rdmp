using Data.Database;
using Data.Dtos;
using Data.Entities;
using Helpers;
using Microsoft.EntityFrameworkCore;

namespace Services;

public interface IRoadmapService
{
    Task<List<Roadmap>> GetRoadmaps();
    Task<Roadmap> GetRoadmap(int id);
    Task<Roadmap> GetRoadmapByUserId(int userId);
    public Task<List<Roadmap>> GetRoadmapsByUserId(int userId);
    Task<Roadmap> AddRoadmap(Roadmap roadmap);
    Task<Roadmap> UpdateRoadmap(int id, Roadmap roadmap);
    Task<bool> DeleteRoadmap(int id);
    Task<TrelloExportResponse> ExportToTrello(int roadmapId, int userId);
}

public class RoadmapService : IRoadmapService
{
    private readonly RdmpContext _context;
    private readonly ITrelloService _trelloService;
    private readonly ITrelloConnectionService _trelloConnectionService;

    public RoadmapService(
        RdmpContext context,
        ITrelloService trelloService,
        ITrelloConnectionService trelloConnectionService)
    {
        _context = context;
        _trelloService = trelloService;
        _trelloConnectionService = trelloConnectionService;
    }

    public async Task<List<Roadmap>> GetRoadmaps()
        => await _context.Roadmaps.ToListAsync();

    public async Task<Roadmap> GetRoadmap(int id)
        => await _context.Roadmaps.FindAsync(id);

    public async Task<Roadmap> GetRoadmapByUserId(int userId)
        => await _context.Roadmaps.FirstOrDefaultAsync(r => r.UserId == userId);

    public async Task<List<Roadmap>> GetRoadmapsByUserId(int userId)
            => await _context.Roadmaps
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Id)
                .AsNoTracking()
                .ToListAsync();

    public async Task<Roadmap> AddRoadmap(Roadmap roadmap)
    {
        _context.Roadmaps.Add(roadmap);

        await _context.SaveChangesAsync();

        return roadmap;
    }

    public async Task<Roadmap> UpdateRoadmap(int id, Roadmap roadmap)
    {
        var existingRoadmap = await _context.Roadmaps.FindAsync(id);
        if (existingRoadmap is null)
        {
            return null;
        }

        existingRoadmap.Content = roadmap.Content;
        existingRoadmap.UserId = roadmap.UserId;

        await _context.SaveChangesAsync();
        return existingRoadmap;
    }

    public async Task<bool> DeleteRoadmap(int id)
    {
        var roadmap = await _context.Roadmaps.FindAsync(id);
        if (roadmap is null)
        {
            return false;
        }

        _context.Roadmaps.Remove(roadmap);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TrelloExportResponse> ExportToTrello(int roadmapId, int userId)
    {
        var roadmap = await _context.Roadmaps
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roadmapId && r.UserId == userId);

        if (roadmap is null)
        {
            throw new KeyNotFoundException("Roadmap not found.");
        }

        if (!await _trelloConnectionService.HasConnection(userId))
        {
            return new TrelloExportResponse
            {
                RequiresAuthorization = true
            };
        }

        var connection = await _trelloConnectionService.GetRequiredConnection(userId);

        var document = RoadmapDeserializer.DeserializeRoadmap(roadmap.Content);
        if (document is null)
        {
            throw new InvalidOperationException("Roadmap content is not in a valid exportable format.");
        }

        var boardName = BuildRandomBoardName();
        var board = await _trelloService.CreateBoard(connection.Token, boardName);

        var skillsList = await _trelloService.CreateList(connection.Token, board.Id, BuildRandomListName());
        foreach (var skill in document.Skills)
        {
            await _trelloService.CreateCard(
                connection.Token,
                skillsList.Id,
                skill.Name,
                BuildSkillDescription(skill));
        }

        var projectsList = await _trelloService.CreateList(connection.Token, board.Id, BuildRandomListName());
        foreach (var project in document.Projects)
        {
            await _trelloService.CreateCard(
                connection.Token,
                projectsList.Id,
                project.Name,
                BuildProjectDescription(project));
        }

        var timelineList = await _trelloService.CreateList(connection.Token, board.Id, BuildRandomListName());
        foreach (var timeline in document.Timeline.OrderBy(t => t.Month))
        {
            await _trelloService.CreateCard(
                connection.Token,
                timelineList.Id,
                $"Month {timeline.Month}",
                BuildTimelineDescription(timeline));
        }

        return new TrelloExportResponse
        {
            BoardId = board.Id,
            BoardName = board.Name,
            BoardUrl = board.Url
        };
    }

    private static string BuildRandomBoardName()
        => $"board-{Guid.NewGuid():N}"[..14];

    private static string BuildRandomListName()
        => $"list-{Guid.NewGuid():N}"[..13];

    private static string BuildSkillDescription(RoadmapSkill skill)
        => BuildDescription(skill.Description, skill.Resources, "Resources");

    private static string BuildProjectDescription(RoadmapProject project)
        => BuildDescription(project.Description, project.Resources, "Resources");

    private static string BuildTimelineDescription(RoadmapTimeline timeline)
    {
        var sections = new List<string>();

        if (timeline.SkillsToLearn.Count > 0)
        {
            sections.Add("Skills to learn");
            sections.AddRange(timeline.SkillsToLearn.Select(skill => $"- {skill}"));
        }

        if (timeline.ProjectsToBuild.Count > 0)
        {
            if (sections.Count > 0)
            {
                sections.Add(string.Empty);
            }

            sections.Add("Projects to build");
            sections.AddRange(timeline.ProjectsToBuild.Select(project => $"- {project}"));
        }

        return sections.Count == 0 ? "No timeline details provided." : string.Join(Environment.NewLine, sections);
    }

    private static string BuildDescription(string description, IReadOnlyCollection<string> items, string sectionTitle)
    {
        var sections = new List<string>();

        if (!string.IsNullOrWhiteSpace(description))
        {
            sections.Add(description.Trim());
        }

        if (items.Count > 0)
        {
            if (sections.Count > 0)
            {
                sections.Add(string.Empty);
            }

            sections.Add(sectionTitle);
            sections.AddRange(items.Select(item => $"- {item}"));
        }

        return sections.Count == 0 ? "No description provided." : string.Join(Environment.NewLine, sections);
    }
}
