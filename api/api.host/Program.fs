module Apex.Api.Host.App

open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Options
open Serilog
open Serilog.Events
open Apex.Api.Model.Configuration

let config = Config.config

Log.Logger <-
    let template = "{Timestamp:yyyy/MM/dd-HH:mm:ss.fff} {Level:u3} {Message:lj}{NewLine}{Properties}{NewLine}{Exception}"

    let mutable logConfig = 
        LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", config.GetValue<LogEventLevel>("Log:Levels:Microsoft"))
            .WriteTo.Console(outputTemplate = template)

    let dir = config.GetValue<string>("Log:Directory")
    if not <| String.IsNullOrEmpty dir
    then
        let logPath = Path.Combine(dir, "server.log")
        logConfig <- logConfig.WriteTo.File(path = logPath, 
                                            outputTemplate = template,
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
        Log.Logger.Information("Configuration: {@config}", config)

        Log.Logger.Information("Starting host.")
        host.Run()
        0
    with
    | ex ->
        Log.Logger.Error(ex, "An unexpected error occurred.")
        -1