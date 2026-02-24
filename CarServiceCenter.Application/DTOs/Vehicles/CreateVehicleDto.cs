namespace CarServiceCenter.Application.DTOs.Vehicles;

public class CreateVehicleDto
{
    public int CustomerId { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}
