module Apex.Api.Host.Config

open Microsoft.Extensions.Configuration

[<CLIMutable>]
type AppOptions = {
    logsDir : string
    webRoot : string
    webAddress : string
    apiAddress : string
    cookieDomain : string
    enableWebServer : bool
    enableWebServerDevelopmentMode : bool
    apexConnectionString : string
}

let config = 
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .AddEnvironmentVariables("APEX_")
        .Build()
        
let options = {
    logsDir = ""
    webRoot = ""
    webAddress = ""
    apiAddress = ""
    cookieDomain = ""
    enableWebServer = false
    enableWebServerDevelopmentMode = false
    apexConnectionString = ""
}
        
do 
    ConfigurationBinder.Bind(config, options)