using Microsoft.Extensions.Options;

namespace App_MicroService_JobScheduler.Services;

public abstract class BaseOptionsSetup<TOption> : IConfigureOptions<TOption>
    where TOption : class
{
    private readonly IConfiguration _configuration;
    private readonly string _sectionName;
    public BaseOptionsSetup(IConfiguration configuration, string sectionName)
    {
        _sectionName = sectionName;
        _configuration = configuration;
    }

    public virtual void Configure(TOption options)
    {
        _configuration.GetSection(_sectionName).Bind(options);
    }
}
