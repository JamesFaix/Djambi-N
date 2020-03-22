module Apex.Api.Host.App

open System.IO
open Microsoft.AspNetCore.Hosting
open Serilog
open Microsoft.Extensions.Configuration

let config = Config.config

Log.Logger <-
    let dir = config.GetValue<string>("logsDir")
    let logPath = Path.Combine(dir, "server.log")
    LoggerConfiguration()
        .WriteTo.File(logPath)
        .WriteTo.Console()
        .CreateLogger()

[<EntryPoint>]
let main _ =
    Log.Logger.Information("Building host.")
    let host = 
        let builder = WebHostBuilder()

        let apiAddress = config.GetValue<string>("apiAddress")
        builder.UseUrls(apiAddress) |> ignore

        builder.UseKestrel() |> ignore

        let enableWebServer = config.GetValue<bool>("enableWebServer")
        if enableWebServer
        then
            let webRoot = config.GetValue<string>("webRoot")
            builder.UseWebRoot(webRoot) |> ignore

        builder.UseStartup<Startup>() |> ignore
        builder.Build()

    // TODO: Log configuration as JSON

    Log.Logger.Information("Starting host.")
    host.Run()
    0