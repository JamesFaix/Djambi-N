module Apex.Api.IntegrationTests.HostFactory

open System
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Model
open Apex.Api.Db.Repositories
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic.Managers
open Apex.Api.Logic.Services

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
                config.AddEnvironmentVariables("APEX_") |> ignore
            )
            .ConfigureServices(fun ctx services -> 
                services.AddDbContext<ApexDbContext>(fun opt -> 
                    let settings = ctx.Configuration.GetSection("Sql");

                    let couldParse, parsedBool = Boolean.TryParse(settings.GetValue<string>("UseSqliteForTesting"))
                    let useSqlite = couldParse && parsedBool
                    
                    if useSqlite
                    then
                        // Must use file, not in-memory Sqlite.
                        // Using in-memory results in foreign key violations when making more than one
                        // SQL command in a test. The data from the first command in saved in a DB instance that
                        // is gone by the time the second command executes.
                        opt.UseSqlite("Filename=Test.db") |> ignore
                    else
                        let cnStr = settings.GetValue<string>("ConnectionString")
                        opt.UseMySql(cnStr) |> ignore
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

    // Make sure the DB is created. This is essential when using Sqlite
    let dbContext = host.Services.GetService<ApexDbContext>()
    ensureDbCreatedSafe dbContext

    Host(host.Services)