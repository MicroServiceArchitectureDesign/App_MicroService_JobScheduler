using App_MicroService_JobScheduler.Models.JobServiceEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App_MicroService_JobScheduler.Models.ModelConfigurations;

public class JobsDetailConfig : IEntityTypeConfiguration<JobsDetail>
{
    public void Configure(EntityTypeBuilder<JobsDetail> builder)
    {
        builder.ToTable(nameof(JobsDetail));
        builder.HasKey(x => x.Id);
    }
}
