using Microsoft.EntityFrameworkCore;
using WebGallery.Data.Entities;

namespace WebGallery.Data;

public class DatabaseContext : DbContext
{
    public DbSet<UserProfiles> UserProfiles { get; set; }
    public DbSet<Artworks> Artworks { get; set; }
    public DbSet<Pictures> Pictures { get; set; }
    public DbSet<ArtworkTags> ArtworkTags { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    private static DbContextOptions GetOptions(string connectionString)
    {
        return new DbContextOptionsBuilder().UseNpgsql(connectionString).Options;
    }
}
