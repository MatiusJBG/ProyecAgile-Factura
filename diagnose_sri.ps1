$baseUrl = "http://localhost:5218"
Write-Host "=== SRI Integration Diagnostic ==="

# 1. Check Certificate
$hasCert = $false
try {
    $cert = Invoke-RestMethod -Uri "$baseUrl/api/certificados/activo" -ErrorAction Stop
    if ($cert) {
        Write-Host "[OK] Active Certificate found: $($cert.nombre)"
        $hasCert = $true
    }
} catch {
    if ($_.Exception.Response.StatusCode -eq [System.Net.HttpStatusCode]::NotFound) {
         Write-Host "[FAIL] No active certificate found. Please upload a .p12 file via the application."
    } else {
         Write-Host "[FAIL] Error checking certificate: $($_.Exception.Message)"
    }
}

# 2. Check Invoices
$hasInvoice = $false
$invoiceId = 0
try {
    $facturasPage = Invoke-RestMethod -Uri "$baseUrl/api/facturas?pageSize=1" -ErrorAction Stop
    # Handle the structure of PagedResult: { items: [], totalCount: ... }
    if ($facturasPage.items -and $facturasPage.items.Count -gt 0) {
        $invoiceId = $facturasPage.items[0].id_Fac
        Write-Host "[OK] Invoice found. ID: $invoiceId"
        $hasInvoice = $true
    } else {
        Write-Host "[FAIL] No invoices found. Please create an invoice first."
    }
} catch {
    Write-Host "[FAIL] Error checking invoices: $($_.Exception.Message)"
}

# 3. Test SRI Sending
if ($hasCert -and $hasInvoice) {
    Write-Host "`n=== Attempting to Send Invoice $invoiceId to SRI ==="
    try {
        $res = Invoke-RestMethod -Method Post -Uri "$baseUrl/api/sri/enviar/$invoiceId" -ErrorAction Stop
        Write-Host "[SUCCESS] Request Sent. Response:" -ForegroundColor Green
        Write-Host ($res | ConvertTo-Json -Depth 10)
    } catch {
        Write-Host "[FAIL] Error sending to SRI: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $details = $reader.ReadToEnd()
            Write-Host "Server Details: $details" -ForegroundColor Red
        }
    }
} else {
    Write-Host "`n[SKIP] Cannot test SRI sending because data is missing (Certificate or Invoice)." -ForegroundColor Yellow
}
