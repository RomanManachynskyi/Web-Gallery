using Microsoft.EntityFrameworkCore;
using WebGallery.Data.Entities;

namespace WebGallery.Data;

public class DatabaseContext : DbContext
{
    public DbSet<UserProfile> UserProfile { get; set; }
    public DbSet<Bookmark> Bookmark { get; set; }
    public DbSet<Like> Like { get; set; }

    public DbSet<Artwork> Artwork { get; set; }
    public DbSet<Picture> Picture { get; set; }
    public DbSet<ArtworkTag> ArtworkTag { get; set; }


    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    private static DbContextOptions GetOptions(string connectionString)
    {
        return new DbContextOptionsBuilder().UseNpgsql(connectionString).Options;
    }
}
