namespace Apex.Api.Host

open System
open System.IO
open System.Linq
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.FileProviders
open Microsoft.OpenApi.Models

open Newtonsoft.Json
open Serilog
open Swashbuckle.AspNetCore.SwaggerGen

open Apex.Api.Common.Json
open Apex.Api.Db
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Repositories
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic.Managers
open Apex.Api.Logic.Services
open Apex.Api.Model
open Apex.Api.Model.Configuration
open Apex.Api.Web
open Apex.Api.Web.Controllers

type Startup() =

    member val Configuration : IConfiguration = (Config.config :> IConfiguration) with get, set

    member __.ConfigureServices(services : IServiceCollection) : unit =
        let configureSwagger (opt : SwaggerGenOptions) : unit =
            let version = "v1"

            let info = OpenApiInfo()
            info.Title <- "Apex API"
            info.Description <- "API for Apex"
            info.Version <- version

            opt.SwaggerDoc(version, info)

            let assemblies = [
                typeof<CreateUserRequest>.Assembly // Model
                typeof<UserController>.Assembly // Controllers
            ]

            for a in assemblies do
                let file = a.GetName().Name + ".xml"
                let path = Path.Combine(AppContext.BaseDirectory, file)
                opt.IncludeXmlComments(path)
            ()

        // Framework services
        services.AddCors() |> ignore
        services.AddControllers() |> ignore

        // Configuration
        services.Configure<AppSettings>(__.Configuration) |> ignore
        services.Configure<SqlSettings>(__.Configuration.GetSection("Sql")) |> ignore
        services.Configure<WebServerSettings>(__.Configuration.GetSection("WebServer")) |> ignore
        services.Configure<ApiSettings>(__.Configuration.GetSection("Api")) |> ignore

        // Serilog
        services.AddSingleton<Serilog.ILogger>(Log.Logger) |> ignore

        // Swagger
        services.AddSwaggerGen(fun opt -> configureSwagger opt) |> ignore

        // Database layer
        services.AddSingleton<CommandContextProvider>() |> ignore
        services.AddSingleton<IEventRepository, EventRepository>() |> ignore
        services.AddSingleton<GameRepository>() |> ignore
        services.AddSingleton<IGameRepository, GameRepository>() |> ignore
        services.AddSingleton<ISearchRepository, SearchRepository>() |> ignore
        services.AddSingleton<ISessionRepository, SessionRepository>() |> ignore
        services.AddSingleton<ISnapshotRepository, SnapshotRepository>() |> ignore
        services.AddSingleton<IUserRepository, UserRepository>() |> ignore

        // Logic layer
        services.AddSingleton<BoardService>() |> ignore
        services.AddSingleton<EventService>() |> ignore
        services.AddSingleton<GameCrudService>() |> ignore
        services.AddSingleton<GameStartService>() |> ignore
        services.AddSingleton<IndirectEffectsService>() |> ignore
        services.AddSingleton<INotificationService, NotificationService>() |> ignore
        services.AddSingleton<PlayerService>() |> ignore
        services.AddSingleton<PlayerStatusChangeService>() |> ignore
        services.AddSingleton<ISessionService, SessionService>() |> ignore
        services.AddSingleton<SelectionOptionsService>() |> ignore
        services.AddSingleton<SelectionService>() |> ignore
        services.AddSingleton<TurnService>() |> ignore
        services.AddSingleton<UserService>() |> ignore
        
        services.AddSingleton<IBoardManager, BoardManager>() |> ignore
        services.AddSingleton<ISearchManager, SearchManager>() |> ignore
        services.AddSingleton<ISessionManager, SessionManager>() |> ignore
        services.AddSingleton<ISnapshotManager, SnapshotManager>() |> ignore
        services.AddSingleton<IUserManager, UserManager>() |> ignore
        // TODO: Break up game manager
        services.AddSingleton<IEventManager, GameManager>() |> ignore
        services.AddSingleton<IGameManager, GameManager>() |> ignore
        services.AddSingleton<IPlayerManager, GameManager>() |> ignore
        services.AddSingleton<ITurnManager, GameManager>() |> ignore

        // Controller layer
        services.AddSingleton<HttpUtility>() |> ignore
        
        ()

    member __.Configure(app : IApplicationBuilder, env : IWebHostEnvironment) : unit =
        let configureNewtonsoft () =
            //This will only provide custom serialization of responses to clients.
            //Custom deserialization of request bodies does not work that same way in this framework.
            //See HttpUtility for deserialization.

            let converters : List<JsonConverter> =
                [
                    OptionJsonConverter()
                    TupleArrayJsonConverter()
                    UnionEnumJsonConverter()
                    SingleFieldUnionJsonConverter()
                ]

            let settings = JsonSerializerSettings()
            settings.Converters <- converters.ToList()
            JsonConvert.DefaultSettings <- (fun () -> settings)

        let configureCors (builder : CorsPolicyBuilder) =
            builder.WithOrigins(__.Configuration.GetValue<string>("Api:WebAddress"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    |> ignore
    
        let configureWebServer(app : IApplicationBuilder) : IApplicationBuilder =
            let isDefault (path : string) =
                path = "/" || path = "/index.html"

            app.UseWhen(
                (fun ctx ->
                    let path = ctx.Request.Path.ToString()
                    let mutable isPublic =
                        path.StartsWith("/resources") ||
                        path.StartsWith("/dist") ||
                        isDefault path

                    if __.Configuration.GetValue<bool>("WebServer:EnableDevelopmentMode") then
                        //This has to be allowed in development because of the React development packages.
                        //TODO: Disable this in release configuration
                        isPublic <- isPublic || path.StartsWith("/node_modules")

                    if isPublic then
                        Log.Logger.Information(sprintf "File: GET %s" path)

                    isPublic
                ),
                (fun a ->
                    let sfOptions = StaticFileOptions()
                    sfOptions.FileProvider <- new PhysicalFileProvider(__.Configuration.GetValue<string>("WebServer:WebRoot"))
            
                    a.UseDefaultFiles()
                        .UseStaticFiles(sfOptions) 
                        |> ignore
                )
            )

        configureNewtonsoft() 

        if __.Configuration.GetValue<bool>("WebServer:Enable")
        then configureWebServer(app) |> ignore

        app.UseCors(configureCors) |> ignore
        app.UseWebSockets() |> ignore

        app.UseRouting() |> ignore
        app.UsePathBase(PathString("/api")) |> ignore
        app.UseEndpoints(fun endpoints -> 
            endpoints.MapControllers() |> ignore
        ) |> ignore

        app.UseMiddleware<ErrorHandlingMiddleware>() |> ignore

        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun opt -> 
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Apex API V1")        
        ) |> ignore
        ()