using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MiniBP.Infrastructure.DataAccess.Postgres;

public class PostgresBpmsDbContextFactory : IDesignTimeDbContextFactory<PostgresBpmsDbContext>
{
    public PostgresBpmsDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PostgresBpmsDbContext>();
        var connectionString = "Host=localhost;Database=MiniBP;Username=postgres;Password=a--1234567";

        builder.UseNpgsql(connectionString);

        return new PostgresBpmsDbContext(builder.Options);
    }
}