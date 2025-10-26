using XYZ.VehiclesService.Domain.Entities;
using XYZ.VehiclesService.Domain.Enums;

namespace XYZ.VehiclesService.Infrastructure.Persistence;

public static class SeedData
{
    public static void Initialize(VehiclesDbContext context)
    {
        // Ensure we have at least two VehicleTypes (insert missing examples)
        var existingTypes = context.VehicleTypes.ToList();
        if (existingTypes.Count < 2)
        {
            var toAdd = new List<VehicleType>();
            if (!existingTypes.Any(t => t.Subtipo == "Camioneta 4x2"))
            {
                toAdd.Add(new VehicleType { TipoMaquinaria = TipoMaquinaria.Liviana, Subtipo = "Camioneta 4x2", TipoMotor = "Gasolina", ConsumoBase = 10, FactorMotor = 1.0, Pavimentado = 1.0, Montanoso = 1.3, Mixto = 1.15 });
            }
            if (!existingTypes.Any(t => t.Subtipo == "Camión mediano"))
            {
                toAdd.Add(new VehicleType { TipoMaquinaria = TipoMaquinaria.Pesada, Subtipo = "Camión mediano", TipoMotor = "Diesel", ConsumoBase = 30, FactorMotor = 0.85, Pavimentado = 1.0, Montanoso = 1.25, Mixto = 1.15 });
            }

            if (toAdd.Any())
            {
                context.VehicleTypes.AddRange(toAdd);
                context.SaveChanges();
            }
            existingTypes = context.VehicleTypes.ToList();
        }
        // Ensure we have at least two Vehicles (insert two example vehicles)
        var existingVehicles = context.Vehicles.ToList();
        if (existingVehicles.Count < 2)
        {
            // pick up to two vehicle types to reference
            var types = context.VehicleTypes.Take(2).ToList();
            if (!types.Any()) return; // nothing to reference

            var vehiclesToAdd = new List<Vehicle>();

            if (existingVehicles.Count == 0)
            {
                vehiclesToAdd.Add(new Vehicle
                {
                    Placa = "ABC-123",
                    Chasis = "CHS-0001",
                    Marca = "MarcaA",
                    Modelo = "ModeloA",
                    Anio = 2018,
                    VehicleTypeId = types[0].Id,
                    Estado = "Operativo",
                    Km = 12345.6,
                    LastMaintenance = DateTime.UtcNow.AddMonths(-3)
                });

                // second vehicle (use second type if exists, otherwise reuse first)
                vehiclesToAdd.Add(new Vehicle
                {
                    Placa = "DEF-456",
                    Chasis = "CHS-0002",
                    Marca = "MarcaB",
                    Modelo = "ModeloB",
                    Anio = 2020,
                    VehicleTypeId = types.Count > 1 ? types[1].Id : types[0].Id,
                    Estado = "Operativo",
                    Km = 5432.1,
                    LastMaintenance = DateTime.UtcNow.AddMonths(-1)
                });
            }
            else if (existingVehicles.Count == 1)
            {
                // add one more vehicle
                vehiclesToAdd.Add(new Vehicle
                {
                    Placa = "DEF-456",
                    Chasis = "CHS-0002",
                    Marca = "MarcaB",
                    Modelo = "ModeloB",
                    Anio = 2020,
                    VehicleTypeId = types.Count > 1 ? types[1].Id : types[0].Id,
                    Estado = "Operativo",
                    Km = 5432.1,
                    LastMaintenance = DateTime.UtcNow.AddMonths(-1)
                });
            }

            if (vehiclesToAdd.Any())
            {
                context.Vehicles.AddRange(vehiclesToAdd);
                context.SaveChanges();
            }
        }
    }
}
