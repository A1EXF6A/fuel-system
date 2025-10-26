using XYZ.VehiclesService.Domain.Enums;

namespace XYZ.VehiclesService.Domain.Entities;

public class VehicleType
{
    public int Id { get; set; }
    public TipoMaquinaria TipoMaquinaria { get; set; }
    public string Subtipo { get; set; } = string.Empty;
    public string TipoMotor { get; set; } = string.Empty;
    public double ConsumoBase { get; set; }
    public double FactorMotor { get; set; }
    public double Pavimentado { get; set; }
    public double Montanoso { get; set; }
    public double Mixto { get; set; }
}
