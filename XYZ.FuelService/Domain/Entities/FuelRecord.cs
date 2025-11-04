using System;
using XYZ.FuelService.Domain.Enums;

namespace XYZ.FuelService.Domain.Entities;

public class FuelRecord
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string VehiclePlaca { get; set; } = string.Empty;
    public int DriverId { get; set; }
    public int RouteId { get; set; }

    public string TipoMaquinaria { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public double EstimatedLiters { get; set; }
    public double ActualLiters { get; set; }
    public EstadoConsumo Estado { get; set; } = EstadoConsumo.Planificado;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
