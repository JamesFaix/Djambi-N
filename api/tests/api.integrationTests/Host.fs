module Apex.Api.IntegrationTests.Host

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

let private builder = 
    HostBuilder()
        .ConfigureAppConfiguration(fun ctx config ->
            config.AddJsonFile("appsettings.json") |> ignore
            config.AddEnvironmentVariables() |> ignore
        )
        .ConfigureServices(fun ctx services -> 
            services.AddDbContext<ApexDbContext>(fun opt -> 
                let cnStr = ctx.Configuration.GetValue<string>("Sql:ConnectionString")
                opt.UseSqlServer(cnStr) |> ignore
                ()
            ) |> ignore

            services.AddTransient<IGameRepository, GameRepository>() |> ignore
            services.AddTransient<IUserRepository, UserRepository>() |> ignore
            services.AddTransient<IEventRepository, EventRepository>() |> ignore
            services.AddTransient<ISearchRepository, SearchRepository>() |> ignore
            services.AddTransient<ISessionRepository, SessionRepository>() |> ignore
            services.AddTransient<ISnapshotRepository, SnapshotRepository>() |> ignore

            services.AddTransient<BoardService>() |> ignore
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
            services.AddTransient<UserService>() |> ignore
                  
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

let private host = builder.Build()

let get<'a>() = host.Services.GetService<'a>()