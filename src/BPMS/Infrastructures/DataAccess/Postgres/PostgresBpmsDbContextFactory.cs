using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace BPMS.Infrastructures.DataAccess.Postgres
{
    public class PostgresBpmsDbContextFactory : IDesignTimeDbContextFactory<PostgresBpmsDbContext>
    {
        public PostgresBpmsDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<PostgresBpmsDbContext>();
            var connectionString = configuration.GetConnectionString("MiniBpDbContext");

            builder.UseNpgsql(connectionString, db => db.UseNetTopologySuite());
            
            return new PostgresBpmsDbContext(builder.Options);
        }
    }
}