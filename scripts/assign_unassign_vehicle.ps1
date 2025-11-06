$base = 'http://localhost:5010'

# Login
$loginBody = '{"Username":"admin","Password":"admin123"}'
Write-Output '=== LOGIN ==='
try {
    $login = Invoke-RestMethod -Method Post -Uri "$base/api/auth/login" -Body $loginBody -ContentType 'application/json' -ErrorAction Stop
    $login | ConvertTo-Json -Depth 5
} catch {
    Write-Output "LOGIN ERROR: $($_.Exception.Message)"
    if ($_.Exception.Response) { try { (New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())).ReadToEnd() } catch {} }
    exit 1
}

$token = $login.token
Write-Output "TOKEN: $token"
$headers = @{ Authorization = "Bearer $token" }

# Assign vehicle to driver 11 (controller expects POST {id}/assign)
$assignUri = "$base/api/drivers/11/assign"
$assignBody = '{"VehiclePlaca":"ABC-123"}'
Write-Output "=== POST $assignUri ==="
try {
    $assignResp = Invoke-RestMethod -Method Post -Uri $assignUri -Headers $headers -Body $assignBody -ContentType 'application/json' -ErrorAction Stop
    Write-Output '=== ASSIGN RESPONSE ==='
    $assignResp | ConvertTo-Json -Depth 5
} catch {
    Write-Output "ASSIGN ERROR: $($_.Exception.Message)"
    if ($_.Exception.Response) { try { (New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())).ReadToEnd() } catch {} }
}

# Unassign vehicle from driver 11 (controller expects POST {id}/unassign with no body)
$unassignUri = "$base/api/drivers/11/unassign"
Write-Output "=== POST $unassignUri ==="
try {
    # No body required by controller for unassign
    $unassignResp = Invoke-RestMethod -Method Post -Uri $unassignUri -Headers $headers -ContentType 'application/json' -ErrorAction Stop
    Write-Output '=== UNASSIGN RESPONSE ==='
    $unassignResp | ConvertTo-Json -Depth 5
} catch {
    Write-Output "UNASSIGN ERROR: $($_.Exception.Message)"
    if ($_.Exception.Response) { try { (New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())).ReadToEnd() } catch {} }
}
