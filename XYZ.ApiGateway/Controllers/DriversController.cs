using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.ApiGateway.Services;

namespace XYZ.ApiGateway.Controllers;
//documentation for DriversController
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DriversController : ControllerBase
{
    private readonly DriversGatewayService _driversService;

    public DriversController(DriversGatewayService driversService)
    {
        _driversService = driversService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDrivers()
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            {
                var username = User.Identity?.Name ?? string.Empty;
                var response = await _driversService.GetAllDriversAsync();
                var driver = response.Drivers.FirstOrDefault(d => d.DocumentNumber == username);
                if (driver == null)
                {
                    return Ok(new object[0]);
                }

                var d = new
                {
                    id = driver.Id,
                    firstName = driver.FirstName,
                    lastName = driver.LastName,
                    documentNumber = driver.DocumentNumber,
                    phoneNumber = driver.PhoneNumber,
                    email = driver.Email,
                    licenseNumber = driver.LicenseNumber,
                    licenseCategory = driver.LicenseCategory,
                    licenseExpiryDate = driver.LicenseExpiryDate.ToDateTime(),
                    driverType = driver.DriverType,
                    status = driver.Status,
                    hireDate = driver.HireDate.ToDateTime(),
                    createdAt = driver.CreatedAt.ToDateTime(),
                    updatedAt = driver.UpdatedAt.ToDateTime(),
                    isAssigned = driver.IsAssigned,
                    assignedVehiclePlaca = driver.AssignedVehiclePlaca,
                    assignmentDate = driver.AssignmentDate?.ToDateTime()
                };

                return Ok(new[] { d });
            }

            var responseAll = await _driversService.GetAllDriversAsync();
            var drivers = responseAll.Drivers.Select(d => new
            {
                id = d.Id,
                firstName = d.FirstName,
                lastName = d.LastName,
                documentNumber = d.DocumentNumber,
                phoneNumber = d.PhoneNumber,
                email = d.Email,
                licenseNumber = d.LicenseNumber,
                licenseCategory = d.LicenseCategory,
                licenseExpiryDate = d.LicenseExpiryDate.ToDateTime(),
                driverType = d.DriverType,
                status = d.Status,
                hireDate = d.HireDate.ToDateTime(),
                createdAt = d.CreatedAt.ToDateTime(),
                updatedAt = d.UpdatedAt.ToDateTime(),
                isAssigned = d.IsAssigned,
                assignedVehiclePlaca = d.AssignedVehiclePlaca,
                assignmentDate = d.AssignmentDate?.ToDateTime()
            }).ToList();

            return Ok(drivers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDriver(int id)
    {
        try
        {
            var response = await _driversService.GetDriverAsync(id);
            var driver = new
            {
                id = response.Driver.Id,
                firstName = response.Driver.FirstName,
                lastName = response.Driver.LastName,
                documentNumber = response.Driver.DocumentNumber,
                phoneNumber = response.Driver.PhoneNumber,
                email = response.Driver.Email,
                licenseNumber = response.Driver.LicenseNumber,
                licenseCategory = response.Driver.LicenseCategory,
                licenseExpiryDate = response.Driver.LicenseExpiryDate.ToDateTime(),
                driverType = response.Driver.DriverType,
                status = response.Driver.Status,
                hireDate = response.Driver.HireDate.ToDateTime(),
                createdAt = response.Driver.CreatedAt.ToDateTime(),
                updatedAt = response.Driver.UpdatedAt.ToDateTime(),
                isAssigned = response.Driver.IsAssigned,
                assignedVehiclePlaca = response.Driver.AssignedVehiclePlaca,
                assignmentDate = response.Driver.AssignmentDate?.ToDateTime()
            };

            return Ok(driver);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableDrivers()
    {
        try
        {
            var response = await _driversService.GetAvailableDriversAsync();
            var drivers = response.Drivers.Select(d => new
            {
                id = d.Id,
                firstName = d.FirstName,
                lastName = d.LastName,
                documentNumber = d.DocumentNumber,
                driverType = d.DriverType,
                licenseCategory = d.LicenseCategory,
                status = d.Status
            }).ToList();

            return Ok(drivers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("by-document/{documentNumber}")]
    public async Task<IActionResult> GetDriverByDocument(string documentNumber)
    {
        try
        {
            var response = await _driversService.GetDriverByDocumentNumberAsync(documentNumber);
            var d = response.Driver;
            var driver = new
            {
                id = d.Id,
                firstName = d.FirstName,
                lastName = d.LastName,
                documentNumber = d.DocumentNumber,
                phoneNumber = d.PhoneNumber,
                email = d.Email,
                licenseNumber = d.LicenseNumber,
                licenseCategory = d.LicenseCategory,
                licenseExpiryDate = d.LicenseExpiryDate.ToDateTime(),
                driverType = d.DriverType,
                status = d.Status,
                hireDate = d.HireDate.ToDateTime(),
                createdAt = d.CreatedAt.ToDateTime(),
                updatedAt = d.UpdatedAt.ToDateTime(),
                isAssigned = d.IsAssigned,
                assignedVehiclePlaca = d.AssignedVehiclePlaca,
                assignmentDate = d.AssignmentDate?.ToDateTime()
            };

            return Ok(driver);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateDriver([FromBody] CreateDriverRequestDto request)
    {
        try
        {
            // Convert string dates to DateTime
            if (!DateTime.TryParse(request.LicenseExpiryDate, out var licenseExpiryDate))
            {
                return BadRequest(new { message = "Invalid license expiry date format" });
            }
            
            if (!DateTime.TryParse(request.HireDate, out var hireDate))
            {
                return BadRequest(new { message = "Invalid hire date format" });
            }

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _driversService.CreateDriverAsync(
                request.FirstName,
                request.LastName,
                request.DocumentNumber,
                request.PhoneNumber,
                request.Email,
                request.LicenseNumber,
                request.LicenseCategory,
                licenseExpiryDate,
                request.DriverType,
                hireDate
            );

            return Ok(new
            {
                success = response.Success,
                driver = new
                {
                    id = response.Driver.Id,
                    firstName = response.Driver.FirstName,
                    lastName = response.Driver.LastName,
                    documentNumber = response.Driver.DocumentNumber,
                    email = response.Driver.Email
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDriver(int id, [FromBody] UpdateDriverRequestDto request)
    {
        try
        {
            // Convert string date to DateTime
            if (!DateTime.TryParse(request.LicenseExpiryDate, out var licenseExpiryDate))
            {
                return BadRequest(new { message = "Invalid license expiry date format" });
            }

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _driversService.UpdateDriverAsync(
                id,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Email,
                request.LicenseNumber,
                request.LicenseCategory,
                licenseExpiryDate,
                request.DriverType,
                request.Status
            );

            return Ok(new
            {
                success = response.Success,
                driver = new
                {
                    id = response.Driver.Id,
                    firstName = response.Driver.FirstName,
                    lastName = response.Driver.LastName,
                    status = response.Driver.Status
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDriver(int id, [FromBody] DeleteDriverRequestDto? request = null)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            // Validar que se proporcione la información requerida
            if (request == null || string.IsNullOrWhiteSpace(request.DeletedBy))
            {
                return BadRequest(new { 
                    message = "DeletedBy field is required",
                    example = new { deletedBy = "admin@company.com", reason = "Optional reason" }
                });
            }

            var response = await _driversService.DeleteDriverAsync(id, request.DeletedBy, request.Reason);
            return Ok(new { success = response.Success });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/assign")]
    public async Task<IActionResult> AssignDriver(int id, [FromBody] AssignDriverRequestDto request)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _driversService.AssignDriverAsync(id, request.VehiclePlaca);
            return Ok(new { success = response.Success });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/unassign")]
    public async Task<IActionResult> UnassignDriver(int id)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _driversService.UnassignDriverAsync(id);
            return Ok(new { success = response.Success });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }






}

public record CreateDriverRequestDto(
    string FirstName,
    string LastName,
    string DocumentNumber,
    string PhoneNumber,
    string Email,
    string LicenseNumber,
    int LicenseCategory,
    string LicenseExpiryDate,
    int DriverType,
    string HireDate
);

public record UpdateDriverRequestDto(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string LicenseNumber,
    int LicenseCategory,
    string LicenseExpiryDate,
    int DriverType,
    int Status
);

public record AssignDriverRequestDto(string VehiclePlaca);

public record DeleteDriverRequestDto(string? DeletedBy, string? Reason);