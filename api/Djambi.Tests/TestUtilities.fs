module TestUtilities

open Microsoft.Extensions.Configuration

open Djambi.Utilities

let private config = 
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .AddJsonFile(Environment.environmentConfigPath(5), false)
        .Build()

let connectionString =
    config.GetConnectionString("Main")
          .Replace("{sqlAddress}", config.["sqlAddress"])
