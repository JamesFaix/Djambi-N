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
    djambiConnectionString : string
}

let config = 
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .AddEnvironmentVariables("DJAMBI_")
        .Build()
        
let options = {
    logsDir = ""
    webRoot = ""
    webAddress = ""
    apiAddress = ""
    cookieDomain = ""
    enableWebServer = false
    enableWebServerDevelopmentMode = false
    djambiConnectionString = ""
}
        
do 
    ConfigurationBinder.Bind(config, options)