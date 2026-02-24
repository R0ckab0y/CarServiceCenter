using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CarServiceCenter.Domain.Entities;

namespace CarServiceCenter.Infrastructure.Configurations;

public class VehicleConfigurations : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.VehicleNumber).IsRequired().HasMaxLength(20);
        builder.HasIndex(x => x.VehicleNumber).IsUnique();

        builder.Property(x => x.Model).IsRequired().HasMaxLength(100);

        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}
