module Apex.Api.Host.Config

open Microsoft.Extensions.Configuration

let config = 
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .AddEnvironmentVariables("APEX_")
        .Build()        