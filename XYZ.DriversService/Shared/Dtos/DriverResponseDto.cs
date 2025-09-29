using XYZ.DriversService.Domain.Enums;

namespace XYZ.DriversService.Shared.Dtos;

public class DriverResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public LicenseCategory LicenseCategory { get; set; }
    public DateTime LicenseExpiryDate { get; set; }
    public DriverType DriverType { get; set; }
    public DriverStatus Status { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsAssigned { get; set; }
    public string? AssignedVehicleId { get; set; }
    public DateTime? AssignmentDate { get; set; }
}