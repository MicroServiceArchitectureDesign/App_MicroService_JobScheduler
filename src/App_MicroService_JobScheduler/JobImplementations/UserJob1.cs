namespace App_MicroService_JobScheduler.JobImplementations;

public class UserJob1 : BaseRecurringExecutableJob, IRecurringExecutableJob
{
    public UserJob1(IRecurringJobManager recurringJobManager) : base(recurringJobManager)
    {
    }

    public string JobName => nameof(UserJob1);

    public string DefaultCron()
    {
        return "*/55 * * * * *";
        //return CronIntervalType.Minute.GetCronExpression(30);
    }

    public void Execute(string cronExpression)
    {
        ExecuteRecurringJob<ITestRepo>(JobName, cronExpression, p => p.TestService("User job 1 called."));
    }
}
