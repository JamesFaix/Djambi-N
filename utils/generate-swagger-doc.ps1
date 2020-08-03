param (
    [string]$buildConfig
)

cd ../api

Write-Host 'Building API...'
dotnet build -c $buildConfig

cd "api.host/bin/$buildConfig/netcoreapp3.1"

Write-Host 'Creating swagger doc...'
dotnet swagger tofile `
    --output '../../../../../utils/swagger.json' `
    'api.host.dll' `
    'v1'

cd "../../../../../utils"