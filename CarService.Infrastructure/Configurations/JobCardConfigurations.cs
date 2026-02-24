using CarServiceCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceCenter.Infrastructure.Configurations;

public class JobCardConfiguration : IEntityTypeConfiguration<JobCard>
{
    public void Configure(EntityTypeBuilder<JobCard> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Complaint)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.Status)
               .IsRequired();

        builder.HasOne(x => x.Vehicle)
               .WithMany()
               .HasForeignKey(x => x.VehicleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ServiceAdvisor)
               .WithMany()
               .HasForeignKey(x => x.ServiceAdvisorId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Mechanic)
               .WithMany()
               .HasForeignKey(x => x.MechanicId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(j => j.VehicleId);
        builder.HasIndex(j => j.ServiceAdvisorId);
        builder.HasIndex(j => j.MechanicId);
        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => j.CreatedDate);
        builder.HasIndex(j => new { j.VehicleId, j.CreatedDate });
    }
}