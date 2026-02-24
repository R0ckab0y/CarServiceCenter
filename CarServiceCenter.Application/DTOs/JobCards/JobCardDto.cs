using CarServiceCenter.Domain.Enums;
namespace CarServiceCenter.Application.DTOs.JobCards;

public class JobCardDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int? MechanicId { get; set; }
    public string Complaint { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
}
