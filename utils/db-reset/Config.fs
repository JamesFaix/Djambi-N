module Apex.Utilities.DbReset.Config

open Microsoft.Extensions.Configuration

[<CLIMutable>]
type AppOptions = {
    sqlRoot : string
    masterConnectionString : string
    apexConnectionString : string
}

let private config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("APEX_")
        .Build()

let options = {
    sqlRoot = ""
    masterConnectionString = ""
    apexConnectionString = ""
}

do 
    ConfigurationBinder.Bind(config, options)