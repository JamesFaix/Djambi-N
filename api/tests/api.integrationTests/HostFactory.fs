module Apex.Api.IntegrationTests.HostFactory

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Apex.Api.Db.Model
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Repositories
open Apex.Api.Logic.Services
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic.Managers
open Microsoft.EntityFrameworkCore
open System
open Serilog

type Host(services : IServiceProvider) =
    member __.Get<'a>() = services.GetService<'a>()

let createHost() =
    let builder = 
        HostBuilder()
            .ConfigureAppConfiguration(fun ctx config ->
                config.AddJsonFile("appsettings.json") |> ignore
            )
            .ConfigureServices(fun ctx services -> 
                services.AddDbContext<ApexDbContext>(fun opt -> 
                    let cnStr = ctx.Configuration.GetValue<string>("Sql:ConnectionString")
                    opt.UseMySql(cnStr) |> ignore
                    ()
                ) |> ignore

                let logger = LoggerConfiguration().CreateLogger()
                services.AddSingleton<ILogger>(logger) |> ignore

                services.AddTransient<IEventRepository, EventRepository>() |> ignore
                services.AddTransient<IGameRepository, GameRepository>() |> ignore
                services.AddTransient<IPlayerRepository, PlayerRepository>() |> ignore
                services.AddTransient<ISearchRepository, SearchRepository>() |> ignore
                services.AddTransient<ISessionRepository, SessionRepository>() |> ignore
                services.AddTransient<ISnapshotRepository, SnapshotRepository>() |> ignore
                services.AddTransient<IUserRepository, UserRepository>() |> ignore
                
                services.AddTransient<IEncryptionService, EncryptionService>() |> ignore
                services.AddTransient<EventService>() |> ignore
                services.AddTransient<GameCrudService>() |> ignore
                services.AddTransient<GameStartService>() |> ignore
                services.AddTransient<IndirectEffectsService>() |> ignore
                services.AddTransient<INotificationService, NotificationService>() |> ignore
                services.AddTransient<PlayerService>() |> ignore
                services.AddTransient<PlayerStatusChangeService>() |> ignore
                services.AddTransient<ISessionService, SessionService>() |> ignore
                services.AddTransient<SelectionOptionsService>() |> ignore
                services.AddTransient<SelectionService>() |> ignore
                services.AddTransient<TurnService>() |> ignore
                  
                services.AddTransient<IBoardManager, BoardManager>() |> ignore
                services.AddTransient<ISearchManager, SearchManager>() |> ignore
                services.AddTransient<ISessionManager, SessionManager>() |> ignore
                services.AddTransient<ISnapshotManager, SnapshotManager>() |> ignore
                services.AddTransient<IUserManager, UserManager>() |> ignore
                // TODO: Break up game manager
                services.AddTransient<IEventManager, GameManager>() |> ignore
                services.AddTransient<IGameManager, GameManager>() |> ignore
                services.AddTransient<IPlayerManager, GameManager>() |> ignore
                services.AddTransient<ITurnManager, GameManager>() |> ignore

                ()
            )

    let host = builder.Build()

    Host(host.Services)