module TestUtilities

open System.IO
open Microsoft.Extensions.Configuration

let private config = 
    (new ConfigurationBuilder() :> IConfigurationBuilder)
        .AddJsonFile("appsettings.json", false)
        .AddJsonFile(Path.GetFullPath("..\\..\\..\\..\\..\\environment.json"), false)
        .Build()

let connectionString =
    config.GetConnectionString("Main")
          .Replace("{sqlAddress}", config.["sqlAddress"])
