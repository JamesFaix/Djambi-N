module Djambi.Utilities.DbReset.Config

open Microsoft.Extensions.Configuration

[<CLIMutable>]
type AppOptions = {
    sqlRoot : string
    masterConnectionString : string
    djambiConnectionString : string
}

let private config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("DJAMBI_")
        .Build()

let options = {
    sqlRoot = ""
    masterConnectionString = ""
    djambiConnectionString = ""
}

do 
    ConfigurationBinder.Bind(config, options)