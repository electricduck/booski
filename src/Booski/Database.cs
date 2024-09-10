using Booski.Common;
using Microsoft.EntityFrameworkCore;

namespace Booski;

public class Database : DbContext
{
    public DbSet<PostLog> PostLogs { get; set; }
    public DbSet<UsernameMap> UsernameMaps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={Program.DbPath}");

    public static void Migrate() {
        var context = new Database();
        context.Database.Migrate();
    }
}