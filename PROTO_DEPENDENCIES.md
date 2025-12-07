# 📋 Matriz de Dependencias de Archivos .proto

## 🎯 **Servicios Propietarios de Contratos**

| Archivo .proto | Servicio Propietario | Versión | Última Actualización |
|---------------|---------------------|---------|---------------------|
| `auth.proto` | XYZ.AuthService | v1.0.0 | 2025-12-07 |
| `drivers.proto` | XYZ.DriversService | v1.2.0 | 2025-12-07 |
| `vehicles.proto` | XYZ.VehiclesService | v1.1.0 | 2025-12-07 |
| `routes.proto` | XYZ.RoutesService | v1.1.0 | 2025-12-07 |
| `fuel.proto` | XYZ.FuelService | v1.0.0 | 2025-12-07 |

## 🔗 **Dependencias entre Servicios**

### AuthService
```
XYZ.AuthService/
└── Protos/
    └── auth.proto (OWNER - Fuente de verdad)
```

### DriversService  
```
XYZ.DriversService/
└── Protos/
    ├── drivers.proto (OWNER - Fuente de verdad)
    └── vehicles.proto (CLIENT - Consume: GetVehicleByPlaca, SetAssignedDriver)
```

### VehiclesService
```
XYZ.VehiclesService/
└── Protos/
    ├── vehicles.proto (OWNER - Fuente de verdad)  
    └── drivers.proto (CLIENT - Consume: GetAvailableDrivers, AssignDriver, UnassignDriver)
```

### RoutesService
```
XYZ.RoutesService/
└── Protos/
    ├── routes.proto (OWNER - Fuente de verdad)
    ├── drivers.proto (CLIENT - Consume: GetDriver, GetDriverByDocumentNumber)
    └── vehicles.proto (CLIENT - Consume: GetVehicleByPlaca)
```

### FuelService
```
XYZ.FuelService/
└── Protos/
    ├── fuel.proto (OWNER - Fuente de verdad)
    ├── routes.proto (CLIENT - Consume: GetRouteById)
    └── vehicles.proto (CLIENT - Consume: GetVehicleByPlaca, GetVehicleTypes)
```

### ApiGateway
```
XYZ.ApiGateway/
└── Protos/
    ├── auth.proto (CLIENT - Consume: Login, Register, ValidateToken)
    ├── drivers.proto (CLIENT - Consume: ALL methods)
    ├── vehicles.proto (CLIENT - Consume: ALL methods)
    ├── routes.proto (CLIENT - Consume: ALL methods)
    └── fuel.proto (CLIENT - Consume: ALL methods)
```

## ⚡ **Métodos Críticos por Servicio**

### Llamadas Inter-Servicios más Comunes:
- **VehiclesService → DriversService**: `GetAvailableDrivers`, `AssignDriver`
- **RoutesService → DriversService**: `GetDriverByDocumentNumber`
- **RoutesService → VehiclesService**: `GetVehicleByPlaca`
- **FuelService → RoutesService**: `GetRouteById`
- **FuelService → VehiclesService**: `GetVehicleByPlaca`, `GetVehicleTypes`

## 🔧 **Proceso de Actualización de Contratos**

### 1. Cambios Compatibles (Safe)
- Agregar nuevos campos opcionales
- Agregar nuevos métodos
- Actualizar solo el servicio propietario

### 2. Cambios No Compatibles (Breaking)
1. **Actualizar servicio propietario** primero
2. **Comunicar cambios** a equipos consumidores
3. **Actualizar servicios consumidores** en este orden:
   - ApiGateway (siempre último)
   - Servicios que dependen del cambio
4. **Verificar integración** completa

### 3. Sincronización Automática Recomendada
```bash
# Script para copiar desde fuente de verdad
./sync-protos.sh drivers
./sync-protos.sh vehicles  
./sync-protos.sh routes
./sync-protos.sh fuel
./sync-protos.sh auth
```

## 🚨 **Reglas de Oro**

1. **NUNCA modificar** archivos .proto que tu servicio NO posee
2. **SIEMPRE versionar** cambios en contratos  
3. **COMUNICAR cambios** breaking con anticipación
4. **MANTENER compatibilidad** hacia atrás cuando sea posible
5. **VALIDAR integración** después de cada cambio

---

**Última sincronización:** 2025-12-07  
**Estado:** ✅ Todos los archivos sincronizados