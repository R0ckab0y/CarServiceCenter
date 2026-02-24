using CarServiceCenter.Domain.Enums;

namespace CarServiceCenter.Domain.Entities;

public class JobCard
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    public int ServiceAdvisorId { get; set; }

    public int? MechanicId { get; set; }

    public string Complaint { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Pending;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedDate { get; set; }

    // Navigation Properties
    public Vehicle? Vehicle { get; set; }
    public User? ServiceAdvisor { get; set; }

    public User? Mechanic { get; set; }
}
