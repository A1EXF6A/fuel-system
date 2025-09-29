using XYZ.DriversService.Domain.Enums;

namespace XYZ.DriversService.Shared.Dtos;

public class CreateDriverRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public LicenseCategory LicenseCategory { get; set; }
    public DateTime LicenseExpiryDate { get; set; }
    public DriverType DriverType { get; set; }
    public DateTime HireDate { get; set; }
}