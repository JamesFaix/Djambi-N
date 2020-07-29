module Apex.Api.Host.App

open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Options
open Newtonsoft.Json
open Serilog
open Serilog.Events
open Apex.Api.Model.Configuration

let config = Config.config

Log.Logger <-
    let mutable logConfig = 
        LoggerConfiguration()
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", config.GetValue<LogEventLevel>("Log:Levels:Microsoft"))

    let dir = config.GetValue<string>("Log:Directory")
    if not <| String.IsNullOrEmpty dir
    then
        let logPath = Path.Combine(dir, "server.log")
        logConfig <- logConfig.WriteTo.File(logPath, 
                                            rollingInterval = RollingInterval.Day, 
                                            retainedFileCountLimit = Nullable(14))

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