using System.ComponentModel.DataAnnotations;

namespace App_MicroService_JobScheduler.Models.JobServiceEntities;

public class JobsDetail
{
    [Key]
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? CronExpression { get; set; }
    public bool IsActive { get; set; }

}