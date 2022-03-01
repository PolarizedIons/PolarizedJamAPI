using Microsoft.EntityFrameworkCore;

namespace PolarizedJam.Database;

public class DatabaseContext : DbContext
{
    public DbSet<Jam> Jams { get; set; } = null!;

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }
}
