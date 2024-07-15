using App_MicroService_JobScheduler.Models.JobServiceEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App_MicroService_JobScheduler.Models.ModelConfigurations;

public class AllowedAdminsConfig : IEntityTypeConfiguration<AllowedAdmin>
{
    public void Configure(EntityTypeBuilder<AllowedAdmin> builder)
    {
        builder.ToTable(nameof(AllowedAdmin));
        builder.HasKey(x => x.Id);
    }
}
