using Data.Database;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public interface IRoadmapService
{
    Task<List<Roadmap>> GetRoadmaps();
    Task<Roadmap> GetRoadmap(int id);
    Task<Roadmap> AddRoadmap(Roadmap roadmap);
    Task<Roadmap> UpdateRoadmap(int id, Roadmap roadmap);
    Task<bool> DeleteRoadmap(int id);
}

public class RoadmapService : IRoadmapService
{
    private readonly RdmpContext _context;

    public RoadmapService(RdmpContext context)
    {
        _context = context;
    }

    public async Task<List<Roadmap>> GetRoadmaps()
        => await _context.Roadmaps.ToListAsync();

    public async Task<Roadmap> GetRoadmap(int id)
        => await _context.Roadmaps.FindAsync(id);

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
}
