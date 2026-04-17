using Microsoft.EntityFrameworkCore;
using MiniBP.BPMS.Domain.Model.Cartable;
using MiniBP.Infrastructure.Helper;

namespace MiniBP.Infrastructure.DataAccess;

public abstract class BpmsDbContext : DbContext
{
    protected readonly string _connectionString;

    protected BpmsDbContext(DbContextOptions options) : base(options) { }

    protected BpmsDbContext(string connectionString)
    {
        _connectionString = connectionString;
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
