$process = $null
try {
    # Start API
    Write-Host 'Starting API...'
    $process = Start-Process `
        -FilePath 'dotnet' `
        -ArgumentList 'run --debug' `
        -WorkingDirectory '../api/api.host' `
        -PassThru
    Write-Host "API process started (ProcessId $($process.Id))..."

    # Download swagger doc w/ retry to wait for app start
    $retryIntervalsInSeconds = @(1, 2, 3, 5, 8)
    $maxRetries = $retryIntervalsInSeconds.count
    $retryCount = 0
    $succeeded = $false

    do {
        try {
            Write-Host 'Trying to download swagger doc...'
            Invoke-WebRequest 'http://localhost:5100/swagger/v1/swagger.json' -OutFile 'swagger.json'
            $succeeded = $true
        }
        catch {
            $seconds = $retryIntervalsInSeconds[$retryCount]
            Write-Host "Failed to download swagger doc. Retrying in $seconds seconds..."
            Start-Sleep -Seconds $seconds
            $retryCount += 1
        }
    }
    while (!$succeeded -and ($retryCount -le $maxRetries))

    if ($succeeded) {
        Write-Host 'Downloaded swagger doc.'
    }
    else {
        Write-Host 'Failed to download swagger doc.'
    }
}
finally {
    # Stop API
    if ($null -ne $process) {
        Write-Host 'Stopping API process...'
        Stop-Process $process -Force
        Write-Host 'API stopped.'
    }
    else {
        Write-Host 'API never started properly. Not stopping.'
    }

}