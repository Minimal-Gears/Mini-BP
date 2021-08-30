using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructures.DataAccess.Postgres
{
    public class PostgresBpmsDbContext:BpmsDbContext
    {
        public PostgresBpmsDbContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.UseIdentityAlwaysColumns();
        }
    }
}