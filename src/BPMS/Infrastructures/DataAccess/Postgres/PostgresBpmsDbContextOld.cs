using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructures.DataAccess.Postgres
{
    public class PostgresBpmsDbContextOld : BpmsDbContext
    {
        public PostgresBpmsDbContextOld(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(connectionString, options => { options.UseNetTopologySuite(); });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.UseIdentityAlwaysColumns();
        }
    }
}