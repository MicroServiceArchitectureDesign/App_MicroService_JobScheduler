namespace App_MicroService_JobScheduler.Contracts;

public interface ISyncService
{
    Task<List<JobsDetail>> SyncJobsInDatabaseAsync(IEnumerable<IRecurringExecutableJob> jobMethods);
}
