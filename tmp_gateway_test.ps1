try {
    $login = Invoke-RestMethod -Uri 'http://localhost:5010/api/auth/login' -Method Post -ContentType 'application/json' -Body '{"Username":"admin","Password":"admin123"}'
} catch {
    Write-Output "LOGIN ERROR: $_"
    exit 1
}
$token = $login.token
Write-Output "TOKEN: $token"

try {
    $val = Invoke-RestMethod -Uri 'http://localhost:5010/api/auth/validate' -Method Post -ContentType 'application/json' -Body (@{ Token = $token } | ConvertTo-Json)
    Write-Output "AUTH_VALIDATE:`n"; Write-Output ($val | ConvertTo-Json -Depth 5)
} catch {
    Write-Output "AUTH_VALIDATE ERROR: $_"
}

function CallAndLog($method, $url, $body=$null) {
    try {
        if ($body) {
            $res = Invoke-RestMethod -Uri $url -Method $method -ContentType 'application/json' -Headers @{ Authorization = "Bearer $token" } -Body $body
        } else {
            $res = Invoke-RestMethod -Uri $url -Method $method -Headers @{ Authorization = "Bearer $token" }
        }
        Write-Output "================================================================"
        Write-Output "$method $url"
        Write-Output ($res | ConvertTo-Json -Depth 6)
        return $res
    } catch {
        Write-Output "ERROR $method $url - $_"
        return $null
    }
}

CallAndLog 'GET' 'http://localhost:5010/api/drivers'
CallAndLog 'GET' 'http://localhost:5010/api/drivers/5'
CallAndLog 'GET' 'http://localhost:5010/api/drivers/available'
CallAndLog 'GET' 'http://localhost:5010/api/vehicles/ABC-123'
CallAndLog 'GET' 'http://localhost:5010/api/routes'
CallAndLog 'GET' 'http://localhost:5010/api/routes/2'
$createRouteBody = @{ Nombre='gw-test'; Origen='quito'; Destino='ambato'; VehiclePlaca='ABC-123'; DriverId=5 } | ConvertTo-Json
CallAndLog 'POST' 'http://localhost:5010/api/routes' $createRouteBody
$createFuelBody = @{ VehiclePlaca='ABC-123'; DriverId=5; RouteId=2 } | ConvertTo-Json
$fuelResp = CallAndLog 'POST' 'http://localhost:5010/api/fuel/plan' $createFuelBody
$reportBody = @{ FilterType='vehicle'; FilterValue='ABC-123' } | ConvertTo-Json
CallAndLog 'POST' 'http://localhost:5010/api/fuel/report' $reportBody
