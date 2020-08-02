if (Test-Path -path "openapi-generator-cli.jar") {
    Write-Host 'OpenAPI Generator CLI already downloaded.'
}
else {
    Write-Host 'Downloading OpenAPI Generator CLI...'
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -OutFile openapi-generator-cli.jar https://repo1.maven.org/maven2/org/openapitools/openapi-generator-cli/4.3.1/openapi-generator-cli-4.3.1.jar
}

Write-Host 'Creating directory...'
New-Item -ItemType Directory -Force -Path ../web2/src/api-client

Write-Host 'Generating TypeScript API client...'
java -jar openapi-generator-cli.jar generate `
    -i .\swagger.json `
    -g typescript-fetch `
    -o ../web2/src/api-client