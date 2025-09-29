namespace XYZ.DriversService.Shared.Dtos;

public class AssignmentRequestDto
{
    public int DriverId { get; set; }
    public string VehicleId { get; set; } = string.Empty;
}