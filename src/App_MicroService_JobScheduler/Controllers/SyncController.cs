using Hangfire;

namespace App_MicroService_JobScheduler.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class SyncController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IRecurringJobManager _recurringJob;
    private readonly ISyncService _syncService;

    public SyncController(IApplicationDbContext context, IRecurringJobManager recurringJob, ISyncService syncService)
    {
        _context = context;
        _recurringJob = recurringJob;
        _syncService = syncService;
    }

    [HttpGet]
    public async Task<IActionResult> SyncJobDetails([FromServices] IEnumerable<IRecurringExecutableJob> jobMethods)
    {
        var newItems = await _syncService.SyncJobsInDatabaseAsync(jobMethods);
        return Ok(newItems);
    }
}



public class JobDetailModel
{
    public long Id { get; set; }
    public string IntervalType { get; set; } = string.Empty;
    public int Interval { get; set; }
    public required string Name { get; set; }
}