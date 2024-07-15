using Hangfire;
using App_MicroService_JobScheduler.Contracts;
using App_MicroService_JobScheduler.Models.Contexts;
using App_MicroService_JobScheduler.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_MicroService_JobScheduler.Controllers;

// [Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class JobDetailController : Controller
{
    private readonly IApplicationDbContext _context;
    private readonly IRecurringJobManager _recurringJob;

    public JobDetailController(IApplicationDbContext context
        , IRecurringJobManager recurringJob)
    {
        _context = context;
        _recurringJob = recurringJob;
    }

    public async Task<IActionResult> Datatable()
    {
        return View(await _context.JobsDetails.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Update(long id)
    {
        var item = await _context.JobsDetails.FindAsync(id);
        return item == null
            ? throw new KeyNotFoundException(nameof(id))
            : (IActionResult)View(new JobDetailModel() { Id = item.Id, Name = item.Name });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateJobIsActiveStatus([FromServices] IEnumerable<IRecurringExecutableJob> jobs, UpdateJobDetailIsActive model)
    {
        var item = await _context.JobsDetails.FindAsync(model.Id) ?? throw new KeyNotFoundException(nameof(model.Id));
        item.IsActive = model.IsActive;
        await _context.CommitAsync();
        if (model.IsActive)
        {
            if (string.IsNullOrEmpty(item.CronExpression))
            {
                throw new InvalidDataException("Data for Cron Expression is null or invalid.");
            }

            var jobMethods = jobs.Where(p => p.GetType().Name == item.Name).SingleOrDefault();
            jobMethods?.Execute(item.CronExpression);
        }
        else
        {
            _recurringJob.RemoveIfExists(item.Name);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromServices] IEnumerable<IRecurringExecutableJob> jobs, JobDetailModel model)
    {
        var cronExpression = model.IntervalType.GetCronExpression(model.Interval);

        var item = await _context.JobsDetails.FindAsync(model.Id) ?? throw new KeyNotFoundException(nameof(model.Id));

        var hasJobCronExpressionChanged = !string.Equals(item.CronExpression, cronExpression, StringComparison.OrdinalIgnoreCase);

        if (hasJobCronExpressionChanged)
        {
            _recurringJob.RemoveIfExists(item.Name);
            var jobMethods = jobs.Where(p => p.GetType().Name == item.Name).SingleOrDefault();
            jobMethods?.Execute(cronExpression);
        }

        item.CronExpression = cronExpression;

        await _context.CommitAsync();
        // HttpContext.Response.Headers.Add("HX-Redirect", "/JobDetail/Datatable");
        return RedirectToAction(nameof(Datatable));
    }
}

public class UpdateJobDetailIsActive
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
}