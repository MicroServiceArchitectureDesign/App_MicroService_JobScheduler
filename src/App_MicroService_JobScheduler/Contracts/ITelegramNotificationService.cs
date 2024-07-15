namespace App_MicroService_JobScheduler.Contracts;

public interface ITelegramNotificationService
{
    Task AlarmTelegramOperatorChannelAsync(string message);
    Task AlarmTelegramTokenChannelAsync(string message);
    Task AlarmTelegramChannelAsync(string message);
    Task AlarmTpLogExceptionChannelAsync(string message);
}
