using XYZ.RoutesService.Domain.Enums;

namespace XYZ.RoutesService.Domain.Entities;

public class Route
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public double DistanciaKm { get; set; }
    public double DuracionMinutos { get; set; }
    public EstadoRuta Estado { get; set; } = EstadoRuta.Planificada;
    public string VehiclePlaca { get; set; } = string.Empty;
    public int DriverId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
