$diff = git diff --stat
if ($null -eq $diff) {
    Write-Host "Found 0 files in diff."
}
else {
    $clientFiles = $diff | Where-Object {$_.Trim().StartsWith("web2/src/api-client/")}

    if ($clientFiles.count -gt 0) {
        Write-Host "Found $($diff.count) files in diff, including $($clientFiles.count) API client files." -ForegroundColor Red

        foreach ($file in $clientFiles) {
            Write-Host $file
            Get-Content $file
        }

        throw "API client has not been regenerated since last API contract changes."
    }
    else {
        Write-Host "Found $($diff.count) files in diff, but no API client files."
    }
}