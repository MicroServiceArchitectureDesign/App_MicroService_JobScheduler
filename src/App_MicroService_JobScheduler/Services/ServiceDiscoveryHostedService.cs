using Consul;

namespace App_MicroService_JobScheduler.Services;

public class ServiceDiscoveryHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger<ServiceDiscoveryHostedService> _logger;

    public ServiceDiscoveryHostedService(IConsulClient consulClient, ILogger<ServiceDiscoveryHostedService> logger)
    {
        _consulClient = consulClient;
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Before Registration Model creation.");
        var registerModel = new AgentServiceRegistration()
        {
            Name =  ServiceDiscoveryConfig.ServiceName,
            ID =  ServiceDiscoveryConfig.ServiceId,
            Address =  ServiceDiscoveryConfig.ServiceAddress.Host,
            Port =  ServiceDiscoveryConfig.ServiceAddress.Port,
        };
        await _consulClient.Agent.ServiceRegister(registerModel, cancellationToken);
        _logger.LogInformation("After Registration in start.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consulClient.Agent.ServiceDeregister(ServiceDiscoveryConfig.ServiceId, cancellationToken);
        _logger.LogInformation("After DeRegistration in stop.");
    }
}


public static class ConsulRegistrationExtensions
{
    public static IServiceCollection AddConsuleService(this IServiceCollection services)
    {
        services.AddHostedService<ServiceDiscoveryHostedService>();
        services.AddSingleton<IConsulClient, ConsulClient>(p=> new ConsulClient(q=> {
            q.Address = ServiceDiscoveryConfig.ServiceDiscoveryAddress;
        }));
        return services;
    }
}
