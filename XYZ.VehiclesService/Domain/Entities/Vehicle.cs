namespace XYZ.VehiclesService.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Chasis { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int VehicleTypeId { get; set; }
    public VehicleType? VehicleType { get; set; }
    public string Estado { get; set; } = "Operativo";
    public double Km { get; set; }
    public DateTime? LastMaintenance { get; set; }
    // Documento del chofer asignado (opcional)
    public string? AssignedDriverDocument { get; set; }
}
