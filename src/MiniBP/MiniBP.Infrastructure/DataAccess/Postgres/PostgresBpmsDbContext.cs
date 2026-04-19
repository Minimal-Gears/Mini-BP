using Microsoft.EntityFrameworkCore;

namespace MiniBP.Infrastructure.DataAccess.Postgres;

public class PostgresBpmsDbContext:BpmsDbContext
{
    public PostgresBpmsDbContext(DbContextOptions options) : base(options)
    {
    }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.UseIdentityAlwaysColumns();
    }
}