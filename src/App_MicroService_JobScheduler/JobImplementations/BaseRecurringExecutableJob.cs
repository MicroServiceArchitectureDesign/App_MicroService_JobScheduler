using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace App_MicroService_JobScheduler.JobImplementations;

public class BaseRecurringExecutableJob
{
    protected static readonly RecurringJobOptions Options = new()
    {
        TimeZone = TimeZoneInfo.Local
    };

    protected readonly IRecurringJobManager _recurringJobManager;

    public BaseRecurringExecutableJob(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public void ExecuteRecurringJob<T>(string jobName, string cronExpression, Expression<Action<T>> methodExpression)
    {
        _recurringJobManager.AddOrUpdate(jobName
            , methodExpression
             , cronExpression
             , Options);
    }
}

public interface ITestRepo
{
    Task TestService(string statement);
}
public class TestRepo : ITestRepo
{
    // private readonly ITelegramNotificationService _telegramNotification;

    // public TestRepo(ITelegramNotificationService telegramNotification)
    // {
    //     _telegramNotification = telegramNotification;
    // }
    public async Task TestService(string statement)
    {
        try
        {
            var currentDate = DateTime.Now;
            Console.WriteLine("-".PadRight(100, '-'));
            Console.WriteLine($"{statement} Job is activly running, current calling datetime : {currentDate}");
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }
    }
}

