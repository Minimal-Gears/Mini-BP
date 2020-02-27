using BPMS.Domain.Model.Cartable;
using BPMS.Infrastructures.Helper;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructures.DataAccess
{
    public abstract class BpmsDbContext : DbContext
    {
        protected readonly string connectionString;

        protected BpmsDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected BpmsDbContext(string connectionString, DbContextOptions options) : base(options)
        {
            this.connectionString = connectionString;
        }

        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseTracker> CaseTrackers { get; set; }
        public DbSet<FlowParameter> FlowParameters { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Case>()
                .HasMany<CaseTracker>(c => c.Tracks).WithOne(ct => ct.Case).HasForeignKey(ct => ct.CaseId);
            modelBuilder.Entity<Case>()
                .HasMany<Note>(c => c.Notes).WithOne(n => n.Case).HasForeignKey(n => n.CaseId);
            modelBuilder.Entity<Case>()
                .HasMany<FlowParameter>(c => c.FlowParameters).WithOne(f => f.Case).HasForeignKey(f => f.CaseId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.AddInterceptors(new LogCommandInterceptor());
#endif
        }
    }
}