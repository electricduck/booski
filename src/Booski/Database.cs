using Booski.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Booski;

public class Database : DbContext
{
    public DbSet<FileCache> FileCaches { get; set; }
    public DbSet<PostLog> PostLogs { get; set; }
    public DbSet<UsernameMap> UsernameMaps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlite($"Data Source={Program.DbPath}")
            .ConfigureWarnings(w =>
                w.Default(WarningBehavior.Ignore)
                .Ignore(RelationalEventId.NonTransactionalMigrationOperationWarning));

    public static void Migrate() {
        var context = new Database();
        context.Database.Migrate();
    }
}