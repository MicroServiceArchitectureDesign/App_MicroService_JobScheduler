using System.Reflection;
using App_MicroService_JobScheduler.Models.JobServiceEntities;

namespace App_MicroService_JobScheduler.Models.Contexts;

public interface IApplicationDbContext
{
    DbSet<JobsDetail> JobsDetails { get; set; }
    DbSet<AllowedAdmin> AllowedAdmins { get; set; }

    Task<int> CommitAsync();
}

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext()
    {

    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<JobsDetail> JobsDetails { get; set; }
    public DbSet<AllowedAdmin> AllowedAdmins { get; set; }

    public async Task<int> CommitAsync()
    {
        return await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}