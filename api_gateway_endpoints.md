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
-d '{"username":"nuevo_usuario","password":"password123","role":"Operador"}'
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
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
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
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

### Drivers Controller

**`GET /api/drivers`** - Obtener todos los conductores
```bash
curl -X GET http://localhost:5010/api/drivers \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`GET /api/drivers/{id}`** - Obtener conductor por ID
```bash
curl -X GET http://localhost:5010/api/drivers/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`GET /api/drivers/available`** - Obtener conductores disponibles
```bash
curl -X GET http://localhost:5010/api/drivers/available \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`GET /api/drivers/by-document/{documentNumber}`** - Obtener conductor por documento
```bash
curl -X GET http://localhost:5010/api/drivers/by-document/12345678 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`POST /api/drivers`** - Crear conductor (Solo Admin)
```bash
curl -X POST http://localhost:5010/api/drivers \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"documentNumber":"87654321","firstName":"Juan","lastName":"Pérez","licenseNumber":"LIC123","phoneNumber":"555-1234","email":"juan.perez@email.com","status":"Active"}'
```

**`PUT /api/drivers/{id}`** - Actualizar conductor (Solo Admin)
```bash
curl -X PUT http://localhost:5010/api/drivers/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"firstName":"Juan Carlos","lastName":"Pérez","phoneNumber":"555-5678","status":"Active"}'
```

**`DELETE /api/drivers/{id}`** - Eliminar conductor (Solo Admin)
```bash
curl -X DELETE http://localhost:5010/api/drivers/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`POST /api/drivers/{id}/assign`** - Asignar conductor a vehículo (Solo Admin)
```bash
curl -X POST http://localhost:5010/api/drivers/1/assign \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"vehicleId":1}'
```

**`POST /api/drivers/{id}/unassign`** - Desasignar conductor (Solo Admin)
```bash
curl -X POST http://localhost:5010/api/drivers/1/unassign \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

### Fuel Controller

**`POST /api/fuel/plan`** - Crear plan de combustible (Solo Admin)
```bash
curl -X POST http://localhost:5010/api/fuel/plan \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"vehicleId":1,"routeId":1,"plannedQuantity":50.5,"plannedDate":"2024-12-06T10:00:00Z"}'
```

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
-d '{"vehicleId":1,"startDate":"2024-12-01T00:00:00Z","endDate":"2024-12-06T23:59:59Z"}'
```

**`GET /api/fuel/reports`** - Obtener todos los reportes
```bash
curl -X GET http://localhost:5010/api/fuel/reports \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
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
-d '{"name":"Ruta Centro","origin":"Plaza Principal","destination":"Terminal Norte","distance":25.5,"estimatedDuration":"01:30:00"}'
```

**`GET /api/routes`** - Obtener todas las rutas
```bash
curl -X GET http://localhost:5010/api/routes \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`GET /api/routes/{id}`** - Obtener ruta por ID
```bash
curl -X GET http://localhost:5010/api/routes/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`PUT /api/routes/{id}`** - Actualizar ruta (Solo Admin)
```bash
curl -X PUT http://localhost:5010/api/routes/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"name":"Ruta Centro Actualizada","distance":28.0,"estimatedDuration":"01:45:00"}'
```

**`DELETE /api/routes/{id}`** - Eliminar ruta (Solo Admin)
```bash
curl -X DELETE http://localhost:5010/api/routes/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`POST /api/routes/{id}/status`** - Actualizar estado de la ruta
```bash
curl -X POST http://localhost:5010/api/routes/1/status \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"status":"InProgress","startTime":"2024-12-06T10:00:00Z"}'
```

### Vehicles Controller

**`GET /api/vehicles/{placa}`** - Obtener vehículo por placa
```bash
curl -X GET http://localhost:5010/api/vehicles/ABC123 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`GET /api/vehicles`** - Obtener todos los vehículos
```bash
curl -X GET http://localhost:5010/api/vehicles \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`POST /api/vehicles`** - Crear vehículo (Solo Admin)
```bash
curl -X POST http://localhost:5010/api/vehicles \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"placa":"XYZ789","brand":"Toyota","model":"Hiace","year":2023,"fuelType":"Gasoline","capacity":15,"fuelTankCapacity":70.0}'
```

**`PUT /api/vehicles/{id}`** - Actualizar vehículo (Solo Admin)
```bash
curl -X PUT http://localhost:5010/api/vehicles/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"brand":"Toyota","model":"Hiace 2024","year":2024,"capacity":16}'
```

**`DELETE /api/vehicles/{id}`** - Eliminar vehículo (Solo Admin)
```bash
curl -X DELETE http://localhost:5010/api/vehicles/1 \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q"
```

**`POST /api/vehicles/{placa}/assign`** - Asignar conductor a vehículo (Solo Admin)
```bash
curl -X POST http://localhost:5010/api/vehicles/ABC123/assign \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY1MDQ5MjYwLCJleHAiOjE3NjUwNTI4NjAsImlhdCI6MTc2NTA0OTI2MH0.tF303IOjbpWSJ07X1z-HItLPKnL9KuMaBTMeJie8F9Q" \
-H "Content-Type: application/json" \
-d '{"driverId":1}'
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