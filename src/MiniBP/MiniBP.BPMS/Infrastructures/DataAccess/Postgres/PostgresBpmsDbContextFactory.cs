using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MiniBP.BPMS.Infrastructures.DataAccess.Postgres;

public class PostgresBpmsDbContextFactory : IDesignTimeDbContextFactory<PostgresBpmsDbContext>
{
    public PostgresBpmsDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PostgresBpmsDbContext>();
        var connectionString = "An appropreate connectionstring would be here soon!";

        builder.UseNpgsql(connectionString, db => db.UseNetTopologySuite());

        return new PostgresBpmsDbContext(builder.Options);
    }
}