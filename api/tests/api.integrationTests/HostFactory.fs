module Djambi.Api.IntegrationTests.HostFactory

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Djambi.Api.Host

type Host(services : IServiceProvider) =
    member __.Get<'a>() = services.GetService<'a>()

let private lockObj = obj()

let private ensureDbCreatedSafe (dbContext : DbContext) =
    lock lockObj (fun () ->
        dbContext.Database.EnsureCreated() |> ignore)

let createHost() =
    let builder = 
        HostBuilder()
            .ConfigureAppConfiguration(fun ctx config ->
                config.AddJsonFile("appsettings.json") |> ignore
                config.AddEnvironmentVariables("DJAMBI_") |> ignore
            )
            .ConfigureServices(fun ctx services -> 
                services |> Startup.AddDbContext ctx.Configuration

                let logger = LoggerConfiguration().CreateLogger()
                services.AddSingleton<ILogger>(logger) |> ignore

                services |> Startup.AddDatabaseLayer
                services |> Startup.AddLogicLayer
            )

    let host = builder.Build()

    // Make sure the DB is created. This is essential when using Sqlite
    let dbContext = host.Services.GetService<DjambiDbContext>()
    ensureDbCreatedSafe dbContext

    Host(host.Services)