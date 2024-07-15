using static App_MicroService_JobScheduler.Utilities.ConstantVariables;

namespace App_MicroService_JobScheduler.Utilities.Extensions;

public static class StringExtensions
{
    public static string GetCronExpression(this string intervalType, int interval)
    {
        return intervalType switch
        {
            CronIntervalType.Second => $"*/{interval} * * * * *",
            CronIntervalType.Minute => $"*/{interval} * * * *",
            CronIntervalType.Hour => $"0 */{interval} * * *",
            CronIntervalType.Day => $"0 0 */{interval} * *",
            CronIntervalType.Month => $"0 0 1 */{interval} *",
            _ => $"*/{interval} * * * *", // default is minute interval.
        };
    }
}


