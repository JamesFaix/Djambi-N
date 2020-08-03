# This script generates the API client code for the web client.
# It must be run after the API is built, and before the web client is built.
# It assumes the API build is outputting a swagger.json file

param (
    [string]$sourcePath,
    [string]$outDir
)

if (Test-Path -path "openapi-generator-cli.jar") {
    Write-Host 'OpenAPI Generator CLI already downloaded.'
}
else {
    Write-Host 'Downloading OpenAPI Generator CLI...'
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -OutFile openapi-generator-cli.jar `
        https://repo1.maven.org/maven2/org/openapitools/openapi-generator-cli/5.0.0-beta/openapi-generator-cli-5.0.0-beta.jar
}

Write-Host 'Deleting old files...'
Remove-Item -LiteralPath $outDir -Force -Recurse

Write-Host 'Creating directory...'
New-Item -ItemType Directory -Force -Path $outDir

Write-Host 'Generating TypeScript API client...'
java -jar openapi-generator-cli.jar generate `
    -i $sourcePath `
    -g typescript-fetch `
    -o $outDir

Write-Host 'Fixing OpenAPI Generator bug w/ TypeScript 3.6...'
$badFile = "$outDir\runtime.ts"
((Get-Content -path $badFile -Raw) -replace 'GlobalFetch','WindowOrWorkerGlobalScope') |
Set-Content -Path $badFile

Write-Host 'API client generated.'
