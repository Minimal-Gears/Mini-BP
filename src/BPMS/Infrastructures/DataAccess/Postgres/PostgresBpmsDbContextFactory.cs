using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BPMS.Infrastructures.DataAccess.Postgres
{
    public class PostgresBpmsDbContextFactory : IDesignTimeDbContextFactory<PostgresBpmsDbContext>
    {
        public PostgresBpmsDbContext CreateDbContext(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine("Environment: " + envName);
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{envName}.json", true);
            var configuration = builder.Build();

           
            
            Console.WriteLine("ConnectionString: PostgresBPMSDataDb/" +
                              configuration.GetConnectionString("MiniBpDbContext"));

            return new PostgresBpmsDbContext(configuration.GetConnectionString("MiniBpDbContext"));
        }
    }
}