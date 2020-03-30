module Apex.Api.Host.Config

open Microsoft.Extensions.Configuration
open System.IO

let config = 
    ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false)
        .AddEnvironmentVariables("APEX_")
        .Build()        