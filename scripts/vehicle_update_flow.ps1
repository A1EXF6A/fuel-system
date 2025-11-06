$base = 'http://localhost:5010'

# Login
$loginBody = @{ Username = 'admin'; Password = 'admin123' } | ConvertTo-Json
Write-Output '=== LOGIN ==='
$login = Invoke-RestMethod -Method Post -Uri "$base/api/auth/login" -Body $loginBody -ContentType 'application/json'
$login | ConvertTo-Json -Depth 5
$token = $login.token
Write-Output "TOKEN: $token"
$headers = @{ Authorization = "Bearer $token" }

# List all vehicles
Write-Output '=== LIST VEHICLES ==='
$all = Invoke-RestMethod -Uri "$base/api/vehicles" -Headers $headers
$all | ConvertTo-Json -Depth 5

# Find vehicle with id = 3 from list
$v = $null
if ($all -ne $null) {
    if ($all.vehicles) { $v = $all.vehicles | Where-Object { $_.id -eq 3 } }
}
Write-Output '=== VEHICLE FROM LIST (id=3) ==='
if ($v) { $v | ConvertTo-Json -Depth 5 } else { Write-Output 'Vehicle with id=3 not found in list' }

# If we have a placa from list, get by placa
if ($v -ne $null -and $v.placa) {
    $placa = $v.placa
    Write-Output "=== GET /api/vehicles/$placa ==="
    try {
        $byPlaca = Invoke-RestMethod -Uri "$base/api/vehicles/$placa" -Headers $headers
        $byPlaca | ConvertTo-Json -Depth 5
    } catch {
        Write-Output "GET by placa error: $($_.Exception.Message)"
        if ($_.Exception.Response) { try { (New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())).ReadToEnd() } catch {} }
    }

    # Prepare update payload using fields from $v
    $update = @{
        Id = 3
        Placa = $placa
        Chasis = ($v.chasis -as [string])
        Marca = ($v.marca -as [string])
        Modelo = ($v.modelo -as [string])
        Anio = 2020
        VehicleTypeId = 1
        Estado = ($v.estado -as [string])
        Km = ($v.km -as [double])
        AssignedDriverDocument = ($v.assignedDriverDocument -as [string])
    }
    $body = $update | ConvertTo-Json
    Write-Output '=== PUT /api/vehicles/3 payload ==='
    Write-Output $body

    try {
        $putResp = Invoke-RestMethod -Method Put -Uri "$base/api/vehicles/3" -Headers $headers -Body $body -ContentType 'application/json'
        Write-Output '=== PUT RESPONSE ==='
        $putResp | ConvertTo-Json -Depth 5
    } catch {
        Write-Output "PUT error: $($_.Exception.Message)"
        if ($_.Exception.Response) { try { (New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())).ReadToEnd() } catch {} }
    }

    # Verify by getting by placa again
    Write-Output "=== GET /api/vehicles/$placa AFTER UPDATE ==="
    try {
        $byPlaca2 = Invoke-RestMethod -Uri "$base/api/vehicles/$placa" -Headers $headers
        $byPlaca2 | ConvertTo-Json -Depth 5
    } catch {
        Write-Output "GET after update error: $($_.Exception.Message)"
        if ($_.Exception.Response) { try { (New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())).ReadToEnd() } catch {} }
    }
} else {
    Write-Output 'Skipping GET/PUT by placa because vehicle with id=3 not found in list.'
}
