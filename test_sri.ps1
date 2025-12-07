try {
    $response = Invoke-RestMethod -Method Post -Uri "http://localhost:5218/api/sri/enviar/1"
    Write-Host "Success:"
    Write-Host ($response | ConvertTo-Json -Depth 5)
} catch {
    Write-Host "Error: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $details = $reader.ReadToEnd()
        Write-Host "Details: $details"
    }
}
