module Djambi.Api.Host.Config

open Microsoft.Extensions.Configuration
open System.IO

let config = 
    ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false)
        .AddEnvironmentVariables("DJAMBI_")
        .Build()        