namespace CarServiceCenter.Application.DTOs.JobCards;

public class CreateJobCardDto
{
    public int VehicleId { get; set; }
    public int ServiceAdvisorId { get; set; }
    public string Complaint { get; set; } = string.Empty;
}
