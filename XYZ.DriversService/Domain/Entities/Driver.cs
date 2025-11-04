using XYZ.DriversService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace XYZ.DriversService.Domain.Entities;

public class Driver
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string DocumentNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(30)]
    public string LicenseNumber { get; set; } = string.Empty;
    
    public LicenseCategory LicenseCategory { get; set; }
    
    public DateTime LicenseExpiryDate { get; set; }
    
    public DriverType DriverType { get; set; }
    
    // Status is nullable so EF Core can distinguish CLR default (null) from DB default value
    public DriverStatus? Status { get; set; }
    
    public DateTime HireDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties for assignments (future use)
    public bool IsAssigned { get; set; } = false;
    
    public string? AssignedVehiclePlaca { get; set; }
    
    public DateTime? AssignmentDate { get; set; }
}