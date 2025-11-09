# 🚀 Fuel System - Microservices Architecture

Sistema de gestión de combustible basado en microservicios para el manejo de choferes, vehículos, rutas y consumo de combustible, con seguridad JWT y separación entre maquinaria liviana y pesada.

## 📋 Tabla de Contenidos

- [Arquitectura del Sistema](#-arquitectura-del-sistema)
- [Estado del Proyecto](#-estado-del-proyecto)
- [Tecnologías](#-tecnologías)
- [Servicios Implementados](#-servicios-implementados)
- [Instalación y Despliegue](#-instalación-y-despliegue)
  - [Despliegue Local con Docker](#-despliegue-local-con-docker)
  - [Despliegue Local sin Docker](#-despliegue-local-sin-docker)
- [Testing](#-testing)
- [Configuración](#-configuración)
- [Próximos Pasos](#-próximos-pasos)

## 🏗️ Arquitectura del Sistema

### Estilo Arquitectónico
- **Patrón**: Microservicios
- **Comunicación**: gRPC + Protocol Buffers
- **Bases de datos**: Independientes por servicio (SQL Server)
- **Despliegue**: Docker, preparado para Kubernetes
- **Logging**: Serilog
- **Seguridad**: JWT Tokens

### Capas por Servicio
Cada microservicio sigue una arquitectura en capas:
```
├── Controllers/        # gRPC Controllers
├── Application/        # Lógica de negocio
├── Domain/            # Entidades e interfaces
├── Infrastructure/    # Acceso a datos y servicios externos
├── Protos/           # Definiciones Protocol Buffers
└── Shared/           # DTOs compartidos
```

## 📊 Estado del Proyecto

### ✅ Completado
- **XYZ.AuthService** - Servicio de Autenticación y Autorización
- **XYZ.DriversService** - Gestión de choferes
- **XYZ.VehiclesService** - Gestión de vehículos
- **XYZ.RoutesService** - Gestión de rutas
- **XYZ.FuelService** - Gestión de consumo de combustible
- **XYZ.ApiGateway** - Proxy REST con control de acceso por roles
- **XYZ.Frontend** - Interfaz de usuario React completa con dashboards por rol

## 🛠️ Tecnologías

### Backend
- **.NET 8.0** - Framework principal
- **gRPC** - Comunicación entre servicios
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **Serilog** - Logging estructurado
- **JWT** - Autenticación y autorización

### Frontend
- **React** - Framework de interfaz de usuario
- **Axios** - Cliente HTTP para APIs REST

### DevOps
- **Docker** - Containerización
- **Docker Compose** - Orquestación local
- **Git** - Control de versiones

### Testing
- **grpcurl** - Testing de servicios gRPC
- **Postman** - Testing de APIs (soporta gRPC)

## 🔐 Servicios Implementados

### AuthService
**Puerto**: `5000`  
**Protocolo**: gRPC (HTTP/2)  
**Base de datos**: `AuthDb`

#### Endpoints disponibles:
- `Auth/Login` - Autenticación de usuarios
- `Auth/Register` - Registro de nuevos usuarios  
- `Auth/ValidateToken` - Validación de tokens JWT
- `Auth/RefreshToken` - Renovación de tokens
- Gestión de usuarios: `ListUsers`, `UpdateUser`, `DeleteUser`

#### Roles del sistema:
- `Admin` - Administrador del sistema (acceso completo)
- `Operador` - Usuario operador (acceso limitado)
- `Supervisor` - Usuario supervisor (acceso limitado)

#### Usuarios de prueba:
- **Admin**: `testuser` / `test123` (acceso completo)
- **Operador**: `operador1` / `oper123` (sin acceso a drivers)
- **Supervisor**: `supervisor1` / `super123` (sin acceso a drivers)

#### Control de Acceso:
- Roles y políticas se gestionan desde Auth; el Gateway propaga Authorization: Bearer <JWT>.

#### Cómo probar (rápido):
- `grpcurl -plaintext -d '{"username":"admin","password":"admin123"}' localhost:5000 Auth/Login`


### DriversService
**Puerto**: `5002`  
**Protocolo**: gRPC (HTTP/2)  
**Base de datos**: `XYZ_DriversDB`

#### Endpoints disponibles:
- `drivers.Drivers/CreateDriver` - Crear nuevo chofer ⚠️ **Solo Admin**
- `drivers.Drivers/GetDriver` - Obtener chofer por ID ⚠️ **Solo Admin**
- `drivers.Drivers/GetAllDrivers` - Listar todos los choferes ⚠️ **Solo Admin**
- `drivers.Drivers/UpdateDriver` - Actualizar datos del chofer ⚠️ **Solo Admin**
- `drivers.Drivers/DeleteDriver` - Eliminar chofer ⚠️ **Solo Admin**
- `drivers.Drivers/GetDriverByDocumentNumber` - Obtener por documento
- `drivers.Drivers/GetAvailableDrivers` - Obtener choferes disponibles ⚠️ **Solo Admin**
- `drivers.Drivers/AssignDriver` - Asignar chofer a vehículo ⚠️ **Solo Admin**
- `drivers.Drivers/UnassignDriver` - Desasignar chofer ⚠️ **Solo Admin**

#### Tipos de chofer:
- `LightMachinery` (1) - Maquinaria ligera
- `HeavyMachinery` (2) - Maquinaria pesada

#### Categorías de licencia:
- `A` (1) - Motocicleta
- `B` (2) - Automóvil
- `C` (3) - Camión
- `D` (4) - Transporte de pasajeros
- `E` (5) - Maquinaria pesada especial

#### Estados del chofer:
- `Active` (1) - Activo
- `Inactive` (2) - Inactivo
- `OnLeave` (3) - De licencia
- `Suspended` (4) - Suspendido

#### Datos iniciales:
- **3 choferes de ejemplo** ya cargados en la base de datos (seed en DriversService)

### VehiclesService
**Puerto**: `5003`  
**Protocolo**: gRPC (HTTP/2)  
**Base de datos**: `XYZ_VehiclesDB` (PostgreSQL en docker-compose)

#### Endpoints disponibles:
- `vehicles.Vehicles/CreateVehicle` - Crear vehículo
- `vehicles.Vehicles/GetVehicleByPlaca` - Obtener vehículo por placa
- `vehicles.Vehicles/SetAssignedDriver` - Asignar/desasignar driver por documento
- `vehicles.Vehicles/GetVehicleById` - Obtener por id
- `vehicles.Vehicles/UpdateVehicle` - Actualizar vehículo
- `vehicles.Vehicles/DeleteVehicle` - Eliminar vehículo
- `vehicles.Vehicles/GetAllVehicles` - Listar todos los vehículos
- `vehicles.Vehicles/GetVehicleTypes` - Listar tipos de vehículo

#### Campos importantes (Create/Update):
- `placa`, `chasis`, `marca`, `modelo`, `anio`, `vehicleTypeId`, `estado`, `km`, `assigned_driver_document`

#### Uso rápido (grpcurl):
- `grpcurl -plaintext -d '{}' localhost:5003 vehicles.Vehicles/GetAllVehicles`
- `grpcurl -plaintext -d '{"placa":"ABC-123","chasis":"CHS-0001","marca":"Toyota","modelo":"Hilux","anio":2020,"vehicleTypeId":1,"estado":"Operativo","km":0.0,"assigned_driver_document":""}' localhost:5003 vehicles.Vehicles/CreateVehicle`

### RoutesService
**Puerto**: `5004`  
**Protocolo**: gRPC (HTTP/2)  
**Base de datos**: `RoutesDb` (PostgreSQL)

#### Endpoints disponibles:
- `routes.Routes/CreateRoute` - Crear ruta
- `routes.Routes/GetRouteById` - Obtener ruta por id
- `routes.Routes/UpdateRoute` - Actualizar ruta
- `routes.Routes/DeleteRoute` - Eliminar ruta
- `routes.Routes/GetAllRoutes` - Listar rutas
- `routes.Routes/UpdateRouteStatus` - Actualizar estado de ruta

#### Campos importantes (Create/Update):
- `nombre`, `origen`, `destino`, `vehicle_placa`, `driverId`, `distanciaKm`, `duracionMinutos`, `estado`

#### Uso rápido (grpcurl):
- `grpcurl -plaintext -d '{}' localhost:5004 routes.Routes/GetAllRoutes`

### FuelService
**Puerto**: `5006`  
**Protocolo**: gRPC (HTTP/2)  
**Base de datos**: `FuelDb` (PostgreSQL)

#### Endpoints disponibles:
- `fuel.Fuel/CreateFuelPlan` - Crear plan de consumo estimado
- `fuel.Fuel/RegisterActualConsumption` - Registrar consumo real
- `fuel.Fuel/GetFuelReport` - Obtener reportes filtrados 
- `fuel.Fuel/GetAllFuelReports` - Obtener todos los reportes
- `fuel.Fuel/UpdateReportStatus` - Actualizar estado de reporte

#### Campos importantes:
- `vehiclePlaca`, `driverId`, `routeId`, `estimatedLiters`, `actualLiters`, `estado`, `tipoMaquinaria`

#### Uso rápido (grpcurl):
- `grpcurl -plaintext -d '{}' localhost:5006 fuel.Fuel/GetAllFuelReports`
- `grpcurl -plaintext -d '{"vehiclePlaca":"ABC-123","driverId":1,"routeId":2}' localhost:5006 Fuel/CreateFuelPlan`


## 🚀 Instalación y Despliegue

### Prerrequisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (para el frontend)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (para despliegue con Docker)
- [SQL Server](https://www.microsoft.com/sql-server) (para despliegue local sin Docker)
- [grpcurl](https://github.com/fullstorydev/grpcurl) (opcional, para testing)

### 🐳 Despliegue Local con Docker

#### 1. Clonar el repositorio
```bash
git clone https://github.com/A1EXF6A/fuel-system.git
cd fuel-system
```

#### 2. Construir y ejecutar con Docker Compose
```bash
# Construir y ejecutar todos los servicios
docker-compose up -d

# Ver logs de los servicios
docker-compose logs -f authservice

# Verificar que los contenedores estén corriendo
docker ps
```

#### 3. Ejecutar el Frontend
```bash
# Instalar dependencias del frontend
cd xyz-frontend
npm install

# Ejecutar el frontend
npm start
```

#### 4. Verificar funcionamiento
```bash
# Verificar conectividad de servicios
grpcurl -plaintext localhost:5000 list

# Probar login
grpcurl -plaintext -d '{"username":"admin","password":"admin123"}' localhost:5000 Auth/Login

# Acceder al frontend en http://localhost:3000
```

#### 5. Detener servicios
```bash
docker-compose down
```

### 💻 Despliegue Local sin Docker

#### 1. Configurar Base de Datos
Actualizar `XYZ.AuthService/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=AuthDb;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

#### 2. Aplicar Migraciones
```bash
cd XYZ.AuthService
dotnet ef database update
```

#### 3. Ejecutar el Servicio
```bash
# Ejecutar en modo desarrollo
dotnet run

# O ejecutar en modo producción
dotnet run --configuration Release
```

#### 4. Verificar funcionamiento
El servicio estará disponible en `http://localhost:5000`

## 🧪 Testing

### Usando grpcurl

#### AuthService (Puerto 5000)
```bash
# Listar servicios disponibles
grpcurl -plaintext localhost:5000 list

# Login con usuario admin
grpcurl -plaintext -d '{"username":"admin","password":"admin123"}' localhost:5000 Auth/Login

# Registrar nuevo usuario
grpcurl -plaintext -d '{"username":"newuser","password":"password123","role":"Operador"}' localhost:5000 Auth/Register
```

#### DriversService (Puerto 5002)
```bash
# Listar servicios disponibles
grpcurl -plaintext localhost:5002 list

# Obtener todos los choferes
grpcurl -plaintext -d '{}' localhost:5002 drivers.Drivers/GetAllDrivers

# Obtener chofer por ID
grpcurl -plaintext -d '{"id":1}' localhost:5002 drivers.Drivers/GetDriver

# Crear nuevo chofer
grpcurl -plaintext -d '{"first_name":"Test","last_name":"Driver","document_number":"99999999","phone_number":"+1234567899","email":"test@company.com","license_number":"LIC999","license_category":2,"license_expiry_date":"2027-12-31T00:00:00Z","driver_type":1,"hire_date":"2024-01-01T00:00:00Z"}' localhost:5002 drivers.Drivers/CreateDriver

# Asignar chofer a vehículo
grpcurl -plaintext -d '{"driver_id":1,"vehicle_placa":"VEH001"}' localhost:5002 drivers.Drivers/AssignDriver
```

> **Nota para Windows**: Si grpcurl no se reconoce, instálalo desde [GitHub Releases](https://github.com/fullstorydev/grpcurl/releases) y agrégalo al PATH del sistema.

### Usando Postman
1. Crear nueva **gRPC Request**
2. **Server URL**: `localhost:5000`
3. Habilitar **"Use server reflection"**
4. Seleccionar servicio **Auth** y método deseado
5. Enviar JSON con los datos requeridos

### Pruebas del Gateway (REST) — ejemplos rápidos

El `ApiGateway` expone una API REST que actúa como proxy hacia los microservicios. Por defecto el gateway se ejecuta en http://localhost:5010 (ver `XYZ.ApiGateway/Program.cs`).

Nota: muchas rutas requieren el header Authorization: Bearer <JWT> obtenido mediante login.

Ejemplos en PowerShell (Windows PowerShell 5.1):

# 1) Obtener token (Login) vía Gateway
```powershell
$body = @{ Username = 'admin'; Password = 'admin123' } | ConvertTo-Json
$resp = Invoke-RestMethod -Method Post -Uri 'http://localhost:5010/api/auth/login' -Body $body -ContentType 'application/json'
$token = $resp.token ?? $resp.Token ?? $resp.accessToken
Write-Host "Token: $token"
```

# 2) Validar token (ejemplo usando body)
```powershell
$validate = @{ Token = $token } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri 'http://localhost:5010/api/auth/validate' -Body $validate -ContentType 'application/json'
```

# 3) Crear vehículo (ejemplo JSON embebido)
```powershell
$json = @{
  Placa = 'ABC-123'
  Chasis = 'CHS-0001'
  Marca = 'Toyota'
  Modelo = 'Hilux'
  Anio = 2020
  VehicleTypeId = 1
  Estado = 'Operativo'
  Km = 0.0
  AssignedDriverDocument = ''
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri 'http://localhost:5010/api/vehicles' -Headers @{ Authorization = "Bearer $token" } -Body $json -ContentType 'application/json'
```

Ejemplos con curl (si prefieres):

```bash
curl -X POST http://localhost:5010/api/auth/login -H "Content-Type: application/json" -d '{"username":"admin","password":"admin123"}'

# Crear vehículo con archivo
curl -X POST http://localhost:5010/api/vehicles -H "Authorization: Bearer <TOKEN>" -H "Content-Type: application/json" -d '{"Placa":"ABC-123","Chasis":"CHS-0001","Marca":"Toyota","Modelo":"Hilux","Anio":2020,"VehicleTypeId":1,"Estado":"Operativo","Km":0.0,"AssignedDriverDocument":""}'
```

Notas importantes:
-- Si recibes errores 401/403, asegúrate de usar el token correcto y que el rol del usuario tenga permisos para la operación.
-- El Gateway suele usar PascalCase en los DTOs; revisa los ejemplos JSON embebidos en esta sección.

### Pruebas individuales de servicios (gRPC) — grpcurl

Para probar directamente cada microservicio (sin pasar por el gateway) se recomienda `grpcurl` con server reflection habilitado en modo Development.

Puertos por servicio (host):
- AuthService: 5000
- DriversService: 5002
- VehiclesService: 5003
- RoutesService: 5004
- FuelService: 5006

Ejemplos básicos con grpcurl:

# 1) Listar servicios en AuthService
```bash
grpcurl -plaintext localhost:5000 list
```

# 2) Login (AuthService)
```bash
grpcurl -plaintext -d '{"username":"admin","password":"admin123"}' localhost:5000 Auth/Login
```

# 3) Validar token (si el servicio dispone del método ValidateToken)
```bash
grpcurl -plaintext -d '{"token":"<TU_TOKEN_AQUI>"}' localhost:5000 Auth/ValidateToken
```

# 4) VehiclesService - Obtener todos los vehículos
```bash
grpcurl -plaintext -d '{}' localhost:5003 vehicles.Vehicles/GetAllVehicles
```

# 5) VehiclesService - Crear vehículo (ejemplo JSON embebido)
```bash
grpcurl -plaintext -d '{"placa":"ABC-123","chasis":"CHS-0001","marca":"Toyota","modelo":"Hilux","anio":2020,"vehicleTypeId":1,"estado":"Operativo","km":0.0,"assigned_driver_document":""}' localhost:5003 vehicles.Vehicles/CreateVehicle
```

# 6) DriversService - Listar choferes
```bash
grpcurl -plaintext -d '{}' localhost:5002 drivers.Drivers/GetAllDrivers
```

# 7) RoutesService - Listar rutas
```bash
grpcurl -plaintext -d '{}' localhost:5004 routes.Routes/GetAllRoutes
```

# 8) FuelService - Ejemplo de consulta
```bash
grpcurl -plaintext -d '{}' localhost:5006 fuel.Fuel/GetAllFuelRecords
```

Consejos:
- En Windows usa la versión binaria de `grpcurl.exe` y abre PowerShell como administrador si hay problemas de permisos.
- Si usas Docker Compose, espera a que los contenedores y la base de datos estén listos antes de ejecutar los comandos. Para las bases Postgres/SQL Server puede tardar unos segundos en estar sanas.

Archivos de ejemplo incluidos en el repositorio:
Nota: los ejemplos de cuerpos JSON para crear/actualizar recursos están embebidos en esta sección del README; no dependemos de ficheros externos.

Si quieres, puedo generar scripts PowerShell listos para ejecutar (uno para el flujo: login -> crear vehículo -> obtener vehículo) o un pequeño conjunto de `curl` y `grpcurl` en `scripts/`.

## ⚙️ Configuración

### Variables de Entorno (Docker)
```yaml
# docker-compose.yml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
```

### Configuración de Base de Datos
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=AuthDb;User=sa;Password=Your_password123;TrustServerCertificate=true;Encrypt=false;"
  }
}
```

### Configuración JWT
```json
{
  "Jwt": {
    "Key": "clave_super_secreta_para_firmar_tokens"
  }
}
```

## � Documentación de Pruebas

Para guías detalladas de testing:
- **AuthService**: Ver ejemplos en la sección Testing arriba
- **DriversService**: Ver `DRIVERS_SERVICE_TESTING.md` para comandos completos

## �🔄 Próximos Pasos

### Funcionalidades Planificadas

#### DriversService ✅ COMPLETADO
- [x] Registrar choferes
- [x] Consultar disponibilidad  
- [x] Asignar choferes por tipo de maquinaria
- [x] Gestión de licencias

#### VehiclesService
- [ ] Clasificación liviano/pesado
- [ ] Estado operativo
- [ ] Asociación con choferes y rutas
- [ ] Especificaciones técnicas

#### RoutesService  
- [ ] Definir rutas con distancias
- [ ] Asignar vehículos y choferes
- [ ] Calcular consumo estimado
- [ ] Gestión de horarios

#### FuelService
- [ ] Registrar consumo real
- [ ] Generar reportes por tipo de maquinaria
- [ ] Comparar estimado vs real
- [ ] Análisis de eficiencia

## 📁 Estructura del Proyecto

## 🗂️ Documentación de Servicios

Aquí encontrarás un resumen por servicio: puertos, RPCs principales, ejemplos de uso y notas de despliegue.

### XYZ.AuthService
- Puerto (host): `5000`
- Protocolo: gRPC (HTTP/2)
- Servicio proto: `Auth`
- RPCs principales:
  - `Login(LoginRequest) returns (AuthResponse)` — obtener JWT
  - `Register(RegisterRequest) returns (AuthResponse)`
  - `ValidateToken(ValidateRequest) returns (ValidateResponse)`
  - `RefreshToken(RefreshRequest) returns (AuthResponse)`
  - `ListUsers`, `UpdateUser`, `DeleteUser`
- Uso rápido (grpcurl):
  - Listar: `grpcurl -plaintext localhost:5000 list`
  - Login: `grpcurl -plaintext -d '{"username":"admin","password":"admin123"}' localhost:5000 Auth/Login`
- Base de datos: SQL Server (ver connection string en `appsettings.json`).
- Cómo arrancar: `cd XYZ.AuthService && dotnet run` o mediante `docker-compose up authservice`.

### XYZ.DriversService
- Puerto (host): `5002` (gRPC)
- Protocolo: gRPC (HTTP/2)
- Servicio proto: `drivers.Drivers`
- RPCs principales:
  - `CreateDriver(CreateDriverRequest)`
  - `GetDriver(GetDriverRequest)`
  - `GetAllDrivers(GetAllDriversRequest)`
  - `UpdateDriver(UpdateDriverRequest)`
  - `DeleteDriver(DeleteDriverRequest)`
  - `GetDriverByDocumentNumber(GetDriverByDocumentNumberRequest)`
  - `GetAvailableDrivers(GetAvailableDriversRequest)`
  - `AssignDriver(AssignDriverRequest)`
  - `UnassignDriver(UnassignDriverRequest)`
- Uso rápido (grpcurl):
  - Listar choferes: `grpcurl -plaintext -d '{}' localhost:5002 drivers.Drivers/GetAllDrivers`
  - Crear chofer: usa el JSON en `create_driver_example` (ver `Protos/drivers.proto` para campos).
- Base de datos: SQL Server (conexión definida en `appsettings.json`).
- Cómo arrancar: `cd XYZ.DriversService && dotnet run` o `docker-compose up driversservice`.

### XYZ.VehiclesService
- Puerto (host): `5003` (gRPC)
- Protocolo: gRPC (HTTP/2)
- Servicio proto: `Vehicles`
- RPCs principales:
  - `CreateVehicle(CreateVehicleRequest)`
  - `GetVehicleByPlaca(GetVehicleByPlacaRequest)`
  - `SetAssignedDriver(SetAssignedDriverRequest)`
  - `GetVehicleById(GetVehicleRequest)`
  - `UpdateVehicle(UpdateVehicleRequest)`
  - `DeleteVehicle(DeleteVehicleRequest)`
  - `GetAllVehicles(EmptyRequest)`
  - `GetVehicleTypes(EmptyRequest)`
- Uso rápido (grpcurl):
  - Obtener todos: `grpcurl -plaintext -d '{}' localhost:5003 vehicles.Vehicles/GetAllVehicles`
  - Crear (ejemplo inline): `grpcurl -plaintext -d '{"placa":"ABC-123","chasis":"CHS-0001","marca":"Toyota","modelo":"Hilux","anio":2020,"vehicleTypeId":1,"estado":"Operativo","km":0.0,"assigned_driver_document":""}' localhost:5003 vehicles.Vehicles/CreateVehicle`
- Base de datos: PostgreSQL (ver `docker-compose.yml` y `appsettings.json`).
- Cómo arrancar: `cd XYZ.VehiclesService && dotnet run` o `docker-compose up vehiclesservice`.

### XYZ.RoutesService
- Puerto (host): `5004` (gRPC)
- Protocolo: gRPC (HTTP/2)
- Servicio proto: `Routes`
- RPCs principales:
  - `CreateRoute(CreateRouteRequest)`
  - `GetRouteById(GetRouteRequest)`
  - `UpdateRoute(UpdateRouteRequest)`
  - `DeleteRoute(DeleteRouteRequest)`
  - `GetAllRoutes(EmptyRequest)`
  - `UpdateRouteStatus(UpdateRouteStatusRequest)`
- Uso rápido (grpcurl):
  - Listar rutas: `grpcurl -plaintext -d '{}' localhost:5004 routes.Routes/GetAllRoutes`
- Base de datos: PostgreSQL.
- Cómo arrancar: `cd XYZ.RoutesService && dotnet run` o `docker-compose up routesservice`.

### XYZ.FuelService
- Puerto (host): `5006` (gRPC)
- Protocolo: gRPC (HTTP/2)
- Servicio proto: `Fuel`
- RPCs principales:
  - `CreateFuelPlan(FuelPlanRequest) returns (FuelPlanResponse)`
  - `RegisterActualConsumption(ActualConsumptionRequest)`
  - `GetFuelReport(FuelReportRequest)`
  - `GetAllFuelReports(EmptyRequest)`
  - `UpdateReportStatus(UpdateReportStatusRequest)`
- Uso rápido (grpcurl):
  - Obtener reportes: `grpcurl -plaintext -d '{}' localhost:5006 fuel.Fuel/GetAllFuelReports`
  - Crear plan: `grpcurl -plaintext -d '{"vehiclePlaca":"ABC-123","driverId":1,"routeId":2}' localhost:5006 Fuel/CreateFuelPlan`
- Base de datos: PostgreSQL.
- Cómo arrancar: `cd XYZ.FuelService && dotnet run` o `docker-compose up fuelservice`.

### XYZ.ApiGateway
- Puerto (host): `5010` (HTTP/1.1 REST)
- Protocolo: HTTP REST (proxy hacia gRPC)
- Rutas REST principales (ejemplos):
  - `POST /api/auth/login` -> reenvía a Auth/Login
  - `POST /api/auth/validate` -> reenvía a Auth/ValidateToken
  - `POST /api/vehicles` -> reenvía a Vehicles/CreateVehicle
  - `GET /api/vehicles` -> listado a Vehicles/GetAllVehicles
- Uso rápido (PowerShell):
  - Login: `Invoke-RestMethod -Method Post -Uri 'http://localhost:5010/api/auth/login' -Body $body -ContentType 'application/json'`
  - Validar token: `Invoke-RestMethod -Method Post -Uri 'http://localhost:5010/api/auth/validate' -Body $validate -ContentType 'application/json'`
- Notas:
  - El gateway valida/propaga Authorization: Bearer <JWT> hacia los servicios.
  - CORS configurado para `http://localhost:3000`.

### Frontend (xyz-frontend / fuel-system-ui)
- El frontend es una aplicación React (puerto por defecto dev: `3000` localmente).
- Consumo: llama al `ApiGateway` para todas las operaciones (login, gestión de vehículos, choferes, rutas, consumo).
- Cómo arrancar:
  - `cd xyz-frontend && npm install && npm start` (dev)
  - Para producción construir y servir con nginx configurado en `fuel-system-ui` Dockerfile.

---


```
fuel-system/
├── docker-compose.yml              # Orquestación de servicios
├── .gitignore                     # Archivos ignorados por Git
├── README.md                      # Este archivo
├── DRIVERS_SERVICE_TESTING.md     # Guía de pruebas DriversService
│
├── XYZ.AuthService/               # Servicio de Autenticación
│   ├── Application/               # Lógica de negocio
│   ├── Controllers/               # Controladores gRPC
│   ├── Domain/                   # Entidades y enums
│   ├── Infrastructure/           # Acceso a datos
│   ├── Migrations/              # Migraciones EF
│   ├── Protos/                  # Protocol Buffers
│   ├── Shared/                  # DTOs compartidos
│   ├── Program.cs               # Punto de entrada
│   ├── Dockerfile              # Imagen Docker
│   └── *.csproj                # Configuración del proyecto
│
└── XYZ.DriversService/            # Servicio de Choferes
    ├── Application/               # Lógica de negocio
    ├── Controllers/               # Controladores gRPC
    ├── Domain/                   # Entidades y enums
    ├── Infrastructure/           # Acceso a datos
    ├── Migrations/              # Migraciones EF
    ├── Protos/                  # Protocol Buffers
    ├── Shared/                  # DTOs compartidos
    ├── Program.cs               # Punto de entrada
    ├── Dockerfile              # Imagen Docker
    └── *.csproj                # Configuración del proyecto
```

## 🤝 Contribución

1. Fork del proyecto
2. Crear feature branch (`git checkout -b feature/nueva-funcionalidad`)
3. Commit cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push al branch (`git push origin feature/nueva-funcionalidad`)
5. Crear Pull Request

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Ver archivo `LICENSE` para más detalles.

---

**Desarrollado por**: [A1EXF6A](https://github.com/A1EXF6A)  
**Proyecto**: Sistema de Gestión de Combustible  
**Fecha**: Septiembre 2025