namespace Data.Database;

using Data.Entities;
using Microsoft.EntityFrameworkCore;

public class RdmpContext : DbContext
{
    public RdmpContext(DbContextOptions<RdmpContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=rdmp.db");
}