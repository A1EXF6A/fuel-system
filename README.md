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

### ⏳ Pendiente
- **XYZ.VehiclesService** - Gestión de vehículos 
- **XYZ.RoutesService** - Gestión de rutas
- **XYZ.FuelService** - Gestión de consumo de combustible

## 🛠️ Tecnologías

### Backend
- **.NET 8.0** - Framework principal
- **gRPC** - Comunicación entre servicios
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **Serilog** - Logging estructurado
- **JWT** - Autenticación y autorización

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

#### Roles del sistema:
- `Admin` - Administrador del sistema (acceso completo)
- `Operador` - Usuario operador (acceso limitado)
- `Supervisor` - Usuario supervisor (acceso limitado)

#### Usuarios de prueba:
- **Admin**: `testuser` / `test123` (acceso completo)
- **Operador**: `operador1` / `oper123` (sin acceso a drivers)
- **Supervisor**: `supervisor1` / `super123` (sin acceso a drivers)

#### Control de Acceso:
- **Servicio de Drivers**: Solo usuarios con rol `Admin` pueden acceder
- **Autenticación**: Todos los roles pueden hacer login
- **Frontend**: Maneja errores 403 para usuarios sin permisos

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
- **3 choferes de ejemplo** ya cargados en la base de datos

## 🚀 Instalación y Despliegue

### Prerrequisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
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

#### 3. Verificar funcionamiento
```bash
# Verificar conectividad
grpcurl -plaintext localhost:5000 list

# Probar login
grpcurl -plaintext -d '{"username":"admin","password":"admin123"}' localhost:5000 Auth/Login
```

#### 4. Detener servicios
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
grpcurl -plaintext -d '{"driver_id":1,"vehicle_id":"VEH001"}' localhost:5002 drivers.Drivers/AssignDriver
```

> **Nota para Windows**: Si grpcurl no se reconoce, instálalo desde [GitHub Releases](https://github.com/fullstorydev/grpcurl/releases) y agrégalo al PATH del sistema.

### Usando Postman
1. Crear nueva **gRPC Request**
2. **Server URL**: `localhost:5000`
3. Habilitar **"Use server reflection"**
4. Seleccionar servicio **Auth** y método deseado
5. Enviar JSON con los datos requeridos

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