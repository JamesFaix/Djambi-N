module Apex.Api.Host.App

open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Serilog
open Microsoft.Extensions.Configuration
open Newtonsoft.Json
open Apex.Api.Model.Configuration
open Microsoft.Extensions.Options

let config = Config.config

Log.Logger <-
    let mutable logConfig = 
        LoggerConfiguration()
            .WriteTo.Console()

    let dir = config.GetValue<string>("Log:Directory")
    if not <| String.IsNullOrEmpty dir
    then
        let logPath = Path.Combine(dir, "server.log")
        logConfig <- logConfig.WriteTo.File(logPath)

    logConfig.CreateLogger()

[<EntryPoint>]
let main _ =
    Log.Logger.Information("Building host.")

    try
        let host = 
            let builder = WebHostBuilder()

            let apiAddress = config.GetValue<string>("Api:ApiAddress")
            builder.UseUrls(apiAddress) |> ignore

            builder.UseKestrel() |> ignore

            let enableWebServer = config.GetValue<bool>("WebServer:Enable")
            if enableWebServer
            then
                let webRoot = config.GetValue<string>("WebServer:WebRoot")
                builder.UseWebRoot(webRoot) |> ignore

            builder.UseDefaultServiceProvider(fun options ->            
                options.ValidateScopes <- true
                options.ValidateOnBuild <- true
                ()
            ) |> ignore

            builder.UseStartup<Startup>() |> ignore
            builder.UseSerilog() |> ignore
            builder.Build()

        let config = host.Services.GetService(typeof<IOptions<AppSettings>>) :?> IOptions<AppSettings> 
                    |> fun x -> x.Value
        let configJson = JsonConvert.SerializeObject(config, Formatting.Indented)
        Log.Logger.Information("Configuration: {config}", configJson)

        Log.Logger.Information("Starting host.")
        host.Run()
        0
    with
    | ex ->
        Log.Logger.Error(ex, "An unexpected error occurred.")
        -1