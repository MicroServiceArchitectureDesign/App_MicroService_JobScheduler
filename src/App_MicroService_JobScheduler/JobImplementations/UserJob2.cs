namespace App_MicroService_JobScheduler.JobImplementations;

public class UserJob2 : BaseRecurringExecutableJob, IRecurringExecutableJob
{
    public UserJob2(IRecurringJobManager recurringJobManager) : base(recurringJobManager)
    {
    }

    public string JobName => nameof(UserJob2);

    public string DefaultCron()
    {
        return "55 * * * * *";
    }

    public void Execute(string cronExpression)
    {
        ExecuteRecurringJob<ITestRepo>(nameof(UserJob2), cronExpression, p => p.TestService("User job 2 called."));
    }
}
