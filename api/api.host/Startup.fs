namespace Apex.Api.Host

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.FileProviders
open Microsoft.OpenApi.Models
open Microsoft.EntityFrameworkCore

open Serilog
open Swashbuckle.AspNetCore.SwaggerGen

open Apex.Api.Db.Interfaces
open Apex.Api.Db.Repositories
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic.Managers
open Apex.Api.Logic.Services
open Apex.Api.Model.Configuration
open Apex.Api.Web
open Apex.Api.Web.Controllers
open Apex.Api.Db.Model
open Apex.Api.Enums
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.IdentityModel.Tokens
open System.Text

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
                typeof<UserController>.Assembly // Model and controllers
                typeof<PlayerKind>.Assembly // Enums
            ]

            for a in assemblies do
                let file = a.GetName().Name + ".xml"
                let path = Path.Combine(AppContext.BaseDirectory, file)
                opt.IncludeXmlComments(path)

        // ASP.NET
        services.AddCors(fun opt ->
            let webAddress = __.Configuration.GetValue<string>("Api:WebAddress")
            opt.AddPolicy("ApiCorsPolicy", fun builder -> 
                builder
                    .WithOrigins(webAddress)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    |> ignore
            )
        ) |> ignore
        services.AddControllers()
            .AddNewtonsoftJson() |> ignore
     
        // JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(fun opt -> 
                let tvps = TokenValidationParameters()
                tvps.ValidateIssuer <- true
                tvps.ValidateAudience <- true
                tvps.ValidateLifetime <- true
                tvps.ValidateIssuerSigningKey <- true
                tvps.ValidIssuer <- "Jwt:Issuer"
                tvps.ValidAudience <- "Jwt:Issuer"
                tvps.IssuerSigningKey <- SymmetricSecurityKey(Encoding.UTF8.GetBytes("Jwt:Key"))
                opt.TokenValidationParameters <- tvps
            ) |> ignore

        // Configuration
        services.Configure<AppSettings>(__.Configuration) |> ignore
        services.Configure<SqlSettings>(__.Configuration.GetSection("Sql")) |> ignore
        services.Configure<WebServerSettings>(__.Configuration.GetSection("WebServer")) |> ignore
        services.Configure<ApiSettings>(__.Configuration.GetSection("Api")) |> ignore

        // Serilog
        services.AddSingleton<Serilog.ILogger>(Log.Logger) |> ignore

        // Swagger
        services.AddSwaggerGen(fun opt -> configureSwagger opt) |> ignore

        // Entity Framework
        services.AddDbContext<ApexDbContext>(fun opt -> 
            let cnStr = __.Configuration.GetValue<string>("Sql:ConnectionString")
            opt.UseSqlServer(cnStr) |> ignore
            ()
        ) |> ignore

        // Anything that depends on DbContext (repositories, services, managers) must be in Scoped or Transient mode

        // Database layer
        services.AddScoped<IEventRepository, EventRepository>() |> ignore
        services.AddScoped<IGameRepository, GameRepository>() |> ignore
        services.AddScoped<ISearchRepository, SearchRepository>() |> ignore
        services.AddScoped<ISessionRepository, SessionRepository>() |> ignore
        services.AddScoped<ISnapshotRepository, SnapshotRepository>() |> ignore
        services.AddScoped<IUserRepository, UserRepository>() |> ignore

        // Logic layer
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

        // Controller layer
        services.AddScoped<CookieProvider>() |> ignore
        services.AddScoped<SessionContextProvider>() |> ignore
        
        ()

    member __.Configure(app : IApplicationBuilder, env : IWebHostEnvironment) : unit =
        // See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1#middleware-order
        // regarding middleware order
    
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

        app.UseMiddleware<ErrorHandlingMiddleware>() |> ignore

        if __.Configuration.GetValue<bool>("WebServer:Enable")
        then configureWebServer(app) |> ignore

        app.UseRouting() |> ignore
        app.UseCors("ApiCorsPolicy") |> ignore

        app.UseWebSockets() |> ignore

        app.UseEndpoints(fun endpoints -> 
            endpoints.MapControllers() |> ignore
        ) |> ignore

        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun opt -> 
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Apex API V1")        
        ) |> ignore
        ()