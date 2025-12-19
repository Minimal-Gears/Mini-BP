using Microsoft.EntityFrameworkCore;

namespace MiniBP.BPMS.Infrastructures.DataAccess.Postgres;

public class PostgresBpmsDbContextOld : BpmsDbContext
{
    public PostgresBpmsDbContextOld(string connectionString) : base(connectionString)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_connectionString, options => { options.UseNetTopologySuite(); });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.UseIdentityAlwaysColumns();
    }
}