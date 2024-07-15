namespace App_MicroService_JobScheduler.JobImplementations;

public class UserJob3 : BaseRecurringExecutableJob, IRecurringExecutableJob
{
    public UserJob3(IRecurringJobManager recurringJobManager) : base(recurringJobManager)
    {
    }

    public string JobName => nameof(UserJob3);

    public string DefaultCron()
    {
        return "*/55 * * * * *";
    }

    public void Execute(string cronExpression)
    {
        ExecuteRecurringJob<ITestRepo>(JobName, cronExpression, p => p.TestService("UserJob3"));
    }
}
