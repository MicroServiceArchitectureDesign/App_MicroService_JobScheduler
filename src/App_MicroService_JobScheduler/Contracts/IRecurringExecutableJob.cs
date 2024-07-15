using System.Text.Json.Serialization;

namespace App_MicroService_JobScheduler.Contracts;

public interface IRecurringExecutableJob
{
    [JsonPropertyOrder(0)]
    string JobName { get; }
    string DefaultCron();

    void Execute(string cronExpression);

}
