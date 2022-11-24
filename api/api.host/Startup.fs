namespace Djambi.Api.Host

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.FileProviders
open Microsoft.OpenApi.Models
open Microsoft.EntityFrameworkCore

open Newtonsoft.Json.Converters
open Serilog
open Swashbuckle.AspNetCore.SwaggerGen

open Djambi.Api.Db.Interfaces
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Logic.Managers
open Djambi.Api.Logic.Services
open Djambi.Api.Model.Configuration
open Djambi.Api.Web
open Djambi.Api.Web.Controllers
open Djambi.Api.Db.Model
open Djambi.Api.Enums

type Startup() =
    static member AddDbContext (config: IConfiguration) (services: IServiceCollection) =
        services.AddDbContext<DjambiDbContext>(fun opt -> 
            let section = config.GetSection("Sql")
            let couldParse, parsedBool = Boolean.TryParse(section.GetValue<string>("UseSqliteForTesting"))
            
            if true then
                // Must use file, not in-memory Sqlite.
                // Using in-memory results in foreign key violations when making more than one
                // SQL command in a test. The data from the first command in saved in a DB instance that
                // is gone by the time the second command executes.
                opt.UseSqlite("Filename=Djambi-Sqlite.db") |> ignore
            else
                let cnStr = section.GetValue<string>("ConnectionString")
                opt.UseMySql(cnStr) |> ignore
        ) |> ignore

    static member AddDatabaseLayer (services: IServiceCollection) =
        services.AddScoped<IEventRepository, EventRepository>() |> ignore
        services.AddScoped<IGameRepository, GameRepository>() |> ignore
        services.AddScoped<IPlayerRepository, PlayerRepository>() |> ignore
        services.AddScoped<ISearchRepository, SearchRepository>() |> ignore
        services.AddScoped<ISessionRepository, SessionRepository>() |> ignore
        services.AddScoped<ISnapshotRepository, SnapshotRepository>() |> ignore
        services.AddScoped<IUserRepository, UserRepository>() |> ignore

    static member AddLogicLayer (services: IServiceCollection) =
        services.AddScoped<IEncryptionService, EncryptionService>() |> ignore
        services.AddScoped<EventService>() |> ignore
        services.AddScoped<GameCrudService>() |> ignore
        services.AddScoped<GameStartService>() |> ignore
        services.AddScoped<IndirectEffectsService>() |> ignore
        services.AddScoped<INotificationService, NotificationService>() |> ignore
        services.AddScoped<PlayerService>() |> ignore
        services.AddScoped<PlayerStatusChangeService>() |> ignore
        services.AddScoped<ISessionService, SessionService>() |> ignore
        services.AddScoped<SelectionOptionsService>() |> ignore
        services.AddScoped<SelectionService>() |> ignore
        services.AddScoped<TurnService>() |> ignore
    
        services.AddScoped<IBoardManager, BoardManager>() |> ignore
        services.AddScoped<ISearchManager, SearchManager>() |> ignore
        services.AddScoped<ISessionManager, SessionManager>() |> ignore
        services.AddScoped<ISnapshotManager, SnapshotManager>() |> ignore
        services.AddScoped<IUserManager, UserManager>() |> ignore
        // TODO: Break up game manager
        services.AddScoped<IEventManager, GameManager>() |> ignore
        services.AddScoped<IGameManager, GameManager>() |> ignore
        services.AddScoped<IPlayerManager, GameManager>() |> ignore
        services.AddScoped<ITurnManager, GameManager>() |> ignore


    member val Configuration : IConfiguration = (Config.config :> IConfiguration) with get, set

    member __.ConfigureServices(services : IServiceCollection) : unit =
        let configureSwagger (opt : SwaggerGenOptions) : unit =
            let version = "v1"

            let info = OpenApiInfo()
            info.Title <- "Djambi-N API"
            info.Description <- "API for Djambi-N"
            info.Version <- version

            opt.SwaggerDoc(version, info)

            let assemblies = [
                typeof<UserController>.Assembly // Model and controllers
                typeof<PlayerKind>.Assembly // Enums
            ]

            for a in assemblies do
                let file = a.GetName().Name + ".xml"
                let path = Path.Combine(AppContext.BaseDirectory, file)
                opt.IncludeXmlComments(path)

        // ASP.NET
        services.AddCors(fun opt ->
            let allowedOrigins = __.Configuration.GetValue<string>("Api:AllowedOrigins").Split(',')
            opt.AddPolicy("ApiCorsPolicy", fun builder -> 
                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    |> ignore
            )
        ) |> ignore
        
        services.AddControllers()
            .AddNewtonsoftJson(fun options -> 
               options.SerializerSettings.Converters.Add(new StringEnumConverter())
            ) |> ignore
        
        services.AddHealthChecks() |> ignore

        // Configuration
        services.Configure<AppSettings>(__.Configuration) |> ignore
        services.Configure<SqlSettings>(__.Configuration.GetSection("Sql")) |> ignore
        services.Configure<WebServerSettings>(__.Configuration.GetSection("WebServer")) |> ignore
        services.Configure<ApiSettings>(__.Configuration.GetSection("Api")) |> ignore

        // Serilog
        services.AddSingleton<Serilog.ILogger>(Log.Logger) |> ignore

        // Swagger
        services.AddSwaggerGen(fun opt -> configureSwagger opt) |> ignore
        services.AddSwaggerGenNewtonsoftSupport() |> ignore

        // Entity Framework
        services |> Startup.AddDbContext __.Configuration

        // Anything that depends on DbContext (repositories, services, managers) must be in Scoped or Transient mode

        services |> Startup.AddDatabaseLayer
        services |> Startup.AddLogicLayer

        // Controller layer
        services.AddScoped<CookieProvider>() |> ignore
        services.AddScoped<SessionContextProvider>() |> ignore
        
        ()

    member __.Configure(app : IApplicationBuilder, env : IWebHostEnvironment) : unit =
        // See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1#middleware-order
        // regarding middleware order
    
        let configureWebServer(app : IApplicationBuilder) : IApplicationBuilder =
            app.UseWhen(
                (fun ctx ->
                    let path = ctx.Request.Path.ToString()
                    let mutable isPublic =
                        path = "/" ||
                        path = "/index.html" ||
                        path = "/bundle.js" ||
                        path = "/bundle.js.map" ||
                        path = "/manifest.json" ||
                        path.StartsWith("/resources")

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

        // Middleware order is very important
        app.UseMiddleware<LoggingMiddelware>() |> ignore
        app.UseMiddleware<ErrorHandlingMiddleware>() |> ignore

        if __.Configuration.GetValue<bool>("Sql:UseSqliteForTesting") then 
            use serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope() 
            let context = serviceScope.ServiceProvider.GetRequiredService<DjambiDbContext>()
            context.Database.EnsureCreated() |> ignore
           
        if __.Configuration.GetValue<bool>("WebServer:Enable")
        then configureWebServer(app) |> ignore

        app.UseRouting() |> ignore
        app.UseCors("ApiCorsPolicy") |> ignore

        app.UseWebSockets() |> ignore

        app.UseEndpoints(fun endpoints -> 
            endpoints.MapControllers() |> ignore
            endpoints.MapHealthChecks("/status") |> ignore
        ) |> ignore

        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun opt -> 
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Djambi API V1")        
        ) |> ignore
        ()