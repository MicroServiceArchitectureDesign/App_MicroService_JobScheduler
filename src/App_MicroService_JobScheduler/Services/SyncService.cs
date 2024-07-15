namespace App_MicroService_JobScheduler.Services;
public class SyncService : ISyncService
{
    private readonly IApplicationDbContext _context;
    private readonly IRecurringJobManager _recurringJob;

    public SyncService(IApplicationDbContext context, IRecurringJobManager recurringJob)
    {
        _context = context;
        _recurringJob = recurringJob;
    }

    public async Task<List<JobsDetail>> SyncJobsInDatabaseAsync(IEnumerable<IRecurringExecutableJob> jobMethods)
    {
        var methodNamesAndCrons = jobMethods.Select(item => new JobsDetail()
        {
            Name = item.JobName,
            CronExpression = item.DefaultCron(),
            IsActive = true
        }).ToList();

        var exitedItems = await _context.JobsDetails.ToListAsync();

        var newItems = methodNamesAndCrons.Where(p => !exitedItems.Select(q => q.Name).Contains(p.Name)).ToList();

        await _context.JobsDetails.AddRangeAsync(newItems);
        await _context.CommitAsync();

        var shouldRunJobs = jobMethods.Where(p => newItems.Select(q => q.Name).Contains(p.JobName)).ToList();

        foreach (var item in shouldRunJobs)
        {
            var cronExpression = newItems.Where(p => p.IsActive && p.Name == item.JobName)
                .Select(p => p.CronExpression)
                .SingleOrDefault();

            if (cronExpression != null)
            {
                _recurringJob.RemoveIfExists(item.JobName);
                item.Execute(cronExpression);
            }
        }

        return newItems;
    }
}
