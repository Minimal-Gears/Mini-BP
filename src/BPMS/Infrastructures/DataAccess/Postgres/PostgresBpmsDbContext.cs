using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructures.DataAccess.Postgres
{
    public class PostgresBpmsDbContext:BpmsDbContext
    {
        public PostgresBpmsDbContext(string connectionString) : base(connectionString)
        {
        }

        public PostgresBpmsDbContext(string connectionString, DbContextOptions options) : base(connectionString, options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.UseNetTopologySuite();
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.UseIdentityAlwaysColumns();
        }
    }
}