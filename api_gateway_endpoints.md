# API Gateway Endpoints

## Endpoints SIN Autenticación Requerida

### Auth Controller

**`POST /api/auth/login`** - Iniciar sesión

```bash
curl -X POST http://localhost:5010/api/auth/login \
-H "Content-Type: application/json" \
-d '{"username":"admin","password":"admin123"}'
```

**`POST /api/auth/register`** - Registrar nuevo usuario

```bash
curl -X POST http://localhost:5010/api/auth/register \
-H "Content-Type: application/json" \
-d '{"username":"Pepe","password":"pepe123","role":"Operador"}'
```

**`POST /api/auth/validate`** - Validar token

```bash
curl -X POST http://localhost:5010/api/auth/validate \
-H "Content-Type: application/json" \
-d '{"token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."}'
```

---

## Endpoints CON Autenticación Requerida

### Auth Controller

**`POST /api/auth/refresh`** - Renovar token

```bash
curl -X POST http://localhost:5010/api/auth/refresh \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"refreshToken":"YWRtaW4="}'
```

**`GET /api/auth/users`** - Listar usuarios

```bash
curl -X GET http://localhost:5010/api/auth/users \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`PUT /api/auth/users/{id}`** - Actualizar usuario

```bash
curl -X PUT http://localhost:5010/api/auth/users/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"username":"usuario_actualizado","role":"Supervisor"}'
```

**`DELETE /api/auth/users/{id}`** - Eliminar usuario

```bash
curl -X DELETE http://localhost:5010/api/auth/users/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

### Drivers Controller

**`GET /api/drivers`** - Obtener todos los conductores

```bash
curl -X GET http://localhost:5010/api/drivers \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`GET /api/drivers/{id}`** - Obtener conductor por ID

```bash
curl -X GET http://localhost:5010/api/drivers/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`GET /api/drivers/available`** - Obtener conductores disponibles

```bash
curl -X GET http://localhost:5010/api/drivers/available \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`GET /api/drivers/by-document/{documentNumber}`** - Obtener conductor por documento

```bash
curl -X GET http://localhost:5010/api/drivers/by-document/12345678 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`POST /api/drivers`** - Crear conductor (Solo Admin)

```bash
curl -X POST http://localhost:5010/api/drivers \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQzMDkwLCJleHAiOjE3NjUxNDY2OTAsImlhdCI6MTc2NTE0MzA5MH0.QpOKzD8uf89J9nFfQ3GMfqhsgnKeZM0zTRFR-5CkCVY" \
-H "Content-Type: application/json" \
-d '{"documentNumber":"87654321","firstName":"Juan","lastName":"Pérez","licenseNumber":"LIC123","phoneNumber":"555-1234","email":"juan.perez@email.com","licenseCategory":1,"licenseExpiryDate":"2025-12-31T00:00:00Z","driverType":1,"hireDate":"2024-01-15T00:00:00Z"}'
```

**Campos requeridos para crear conductor:**
- `documentNumber`: string - Número de documento de identidad
- `firstName`: string - Nombres
- `lastName`: string - Apellidos  
- `licenseNumber`: string - Número de licencia de conducir
- `phoneNumber`: string - Teléfono
- `email`: string - Correo electrónico
- `licenseCategory`: int - Categoría de licencia (0=A1, 1=A2, 2=B1, 3=B2, 4=C1, 5=C2, 6=D1, 7=D2)
- `licenseExpiryDate`: string - Fecha de vencimiento de licencia (formato ISO 8601)
- `driverType`: int - Tipo de conductor (0=Temporal, 1=Permanente)
- `hireDate`: string - Fecha de contratación (formato ISO 8601)

**`PUT /api/drivers/{id}`** - Actualizar conductor (Solo Admin)

```bash
curl -X PUT http://localhost:5010/api/drivers/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"firstName":"Juan Carlos","lastName":"Pérez","phoneNumber":"555-5678","email":"juan.carlos@email.com","licenseNumber":"LIC123","licenseCategory":1,"licenseExpiryDate":"2025-12-31T00:00:00Z","driverType":1,"status":1}'
```

**Campos requeridos para actualizar conductor:**
- `firstName`: string - Nombres
- `lastName`: string - Apellidos
- `phoneNumber`: string - Teléfono
- `email`: string - Correo electrónico
- `licenseNumber`: string - Número de licencia de conducir
- `licenseCategory`: int - Categoría de licencia (0=A1, 1=A2, 2=B1, 3=B2, 4=C1, 5=C2, 6=D1, 7=D2)
- `licenseExpiryDate`: string - Fecha de vencimiento de licencia (formato ISO 8601)
- `driverType`: int - Tipo de conductor (0=Temporal, 1=Permanente)
- `status`: int - Estado del conductor (1=Active, 2=Inactive, 3=OnLeave, 4=Suspended)

**`DELETE /api/drivers/{id}`** - Eliminar conductor (Solo Admin)

```bash
curl -X DELETE http://localhost:5010/api/drivers/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`POST /api/drivers/{id}/assign`** - Asignar conductor a vehículo (Solo Admin)

```bash
curl -X POST http://localhost:5010/api/drivers/1/assign \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"vehiclePlaca":"ABC123"}'
```

**Campos requeridos para asignar conductor:**
- `vehiclePlaca`: string - Placa del vehículo al cual asignar el conductor

**`POST /api/drivers/{id}/unassign`** - Desasignar conductor (Solo Admin)

```bash
curl -X POST http://localhost:5010/api/drivers/1/unassign \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

### Fuel Controller

**`POST /api/fuel/plan`** - Crear plan de combustible (Solo Admin)

```bash
curl -X POST http://localhost:5010/api/fuel/plan \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"vehiclePlaca":"ABC123","driverId":1,"routeId":1}'
```

**Campos requeridos para crear plan de combustible:**
- `vehiclePlaca`: string - Placa del vehículo
- `driverId`: int - ID del conductor asignado
- `routeId`: int - ID de la ruta planificada

**`POST /api/fuel/register`** - Registrar consumo real

```bash
curl -X POST http://localhost:5010/api/fuel/register \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"planId":1,"actualQuantity":48.2,"actualDate":"2024-12-06T10:30:00Z","notes":"Consumo menor al planificado"}'
```

**`POST /api/fuel/report`** - Obtener reporte de combustible

```bash
curl -X POST http://localhost:5010/api/fuel/report \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"filterType":"vehicle","filterValue":"ABC123"}'
```

**Campos requeridos para obtener reporte de combustible:**
- `filterType`: string - Tipo de filtro (ej: "vehicle", "driver", "route")
- `filterValue`: string - Valor del filtro (ej: placa del vehículo, ID del conductor, ID de la ruta)

**`GET /api/fuel/reports`** - Obtener todos los reportes

```bash
curl -X GET http://localhost:5010/api/fuel/reports \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`POST /api/fuel/reports/{id}/status`** - Actualizar estado del reporte

```bash
curl -X POST http://localhost:5010/api/fuel/reports/1/status \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"status":"Approved","comments":"Reporte revisado y aprobado"}'
```

### Routes Controller

**`POST /api/routes`** - Crear ruta (Solo Admin)

```bash
curl -X POST http://localhost:5010/api/routes \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"Nombre":"Ruta Centro","Origen":"Plaza Principal","Destino":"Terminal Norte","VehiclePlaca":"ABC123","DriverId":1}'
```

**`GET /api/routes`** - Obtener todas las rutas

```bash
curl -X GET http://localhost:5010/api/routes \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`GET /api/routes/{id}`** - Obtener ruta por ID

```bash
curl -X GET http://localhost:5010/api/routes/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`PUT /api/routes/{id}`** - Actualizar ruta (Solo Admin)

```bash
curl -X PUT http://localhost:5010/api/routes/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"Nombre":"Ruta Centro Actualizada","Origen":"Plaza Principal","Destino":"Terminal Norte","VehiclePlaca":"ABC123","DriverId":1}'
```

**`DELETE /api/routes/{id}`** - Eliminar ruta (Solo Admin)

```bash
curl -X DELETE http://localhost:5010/api/routes/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`POST /api/routes/{id}/status`** - Actualizar estado de la ruta

```bash
curl -X POST http://localhost:5010/api/routes/1/status \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"Estado":"InProgress"}'
```

### Vehicles Controller

**`GET /api/vehicles/{placa}`** - Obtener vehículo por placa

```bash
curl -X GET http://localhost:5010/api/vehicles/ABC123 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`GET /api/vehicles`** - Obtener todos los vehículos

```bash
curl -X GET http://localhost:5010/api/vehicles \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`POST /api/vehicles`** - Crear vehículo (Solo Admin)

```bash
curl -X POST http://localhost:5010/api/vehicles \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"Placa":"XYZ789","Chasis":"CHASIS123","Brand":"Toyota","Model":"Hiace","Year":2023,"FuelType":"Gasoline","Capacity":15,"FuelTankCapacity":70.0,"VehicleTypeId":1,"Estado":"Disponible","Km":0,"AssignedDriverDocument":null}'
```

**`PUT /api/vehicles/{id}`** - Actualizar vehículo (Solo Admin)

```bash
curl -X PUT http://localhost:5010/api/vehicles/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"Id":1,"Placa":"XYZ789","Chasis":"CHASIS123","Brand":"Toyota","Model":"Hiace 2024","Year":2024,"FuelType":"Gasoline","Capacity":16,"FuelTankCapacity":70.0,"VehicleTypeId":1,"Estado":"Disponible","Km":5000,"AssignedDriverDocument":null}'
```

**`DELETE /api/vehicles/{id}`** - Eliminar vehículo (Solo Admin)

```bash
curl -X DELETE http://localhost:5010/api/vehicles/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MTQyNzk4LCJleHAiOjE3NjUxNDYzOTgsImlhdCI6MTc2NTE0Mjc5OH0.65DfEZhYTjRVoGLkXRtGRZbAVRCgmzX52vWWuxnY_fk"
```

**`POST /api/vehicles/{placa}/assign`** - Asignar conductor a vehículo (Solo Admin)

```bash
curl -X POST http://localhost:5010/api/vehicles/ABC123/assign \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"DriverDocument":"12345678"}'
```

---

## Notas de Autorización por Rol

### Admin

- Acceso completo a todos los endpoints
- Puede crear, actualizar y eliminar recursos

### Supervisor

- Acceso a consultas y reportes
- Puede actualizar estados de rutas y reportes de combustible
- No puede crear/modificar/eliminar recursos

### Operador

- Acceso limitado solo a información de su vehículo asignado
- Puede ver solo su información como conductor
- Puede registrar consumo de combustible de su vehículo
- Puede actualizar estados de rutas y reportes de su vehículo
- Puede ver reportes solo de su vehículo

### Endpoints que varían según el rol:

- **Drivers**: Operadores solo ven su propia información
- **Vehicles**: Operadores solo ven su vehículo asignado
- **Routes**: Operadores solo ven rutas de su vehículo
- **Fuel**: Operadores solo pueden operar con datos de su vehículo

