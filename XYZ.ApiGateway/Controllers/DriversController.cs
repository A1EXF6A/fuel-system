using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.ApiGateway.Services;

namespace XYZ.ApiGateway.Controllers;
//documentation for DriversController
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
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
            var response = await _driversService.GetAllDriversAsync();
            var drivers = response.Drivers.Select(d => new
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
                assignedVehicleId = d.AssignedVehicleId,
                assignmentDate = d.AssignmentDate?.ToDateTime(),
                isDeleted = d.IsDeleted,
                deletedAt = d.DeletedAt?.ToDateTime(),
                deletedBy = d.DeletedBy,
                deletionReason = d.DeletionReason
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
                assignedVehicleId = response.Driver.AssignedVehicleId,
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
            var response = await _driversService.AssignDriverAsync(id, request.VehicleId);
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
            var response = await _driversService.UnassignDriverAsync(id);
            return Ok(new { success = response.Success });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreDriver(int id)
    {
        try
        {
            var response = await _driversService.RestoreDriverAsync(id);
            return Ok(new { success = response.Success });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeletedDrivers()
    {
        try
        {
            var response = await _driversService.GetDeletedDriversAsync();
            var drivers = response.Drivers.Select(d => new
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
                assignedVehicleId = d.AssignedVehicleId,
                assignmentDate = d.AssignmentDate?.ToDateTime(),
                isDeleted = d.IsDeleted,
                deletedAt = d.DeletedAt?.ToDateTime(),
                deletedBy = d.DeletedBy,
                deletionReason = d.DeletionReason
            }).ToList();

            return Ok(drivers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}/permanent")]
    public async Task<IActionResult> HardDeleteDriver(int id)
    {
        try
        {
            var response = await _driversService.HardDeleteDriverAsync(id);
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

public record AssignDriverRequestDto(string VehicleId);

public record DeleteDriverRequestDto(string? DeletedBy, string? Reason);