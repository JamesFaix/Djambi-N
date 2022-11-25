namespace Djambi.Api.Model.Configuration

open Serilog.Events

[<CLIMutable>]
type SqlSettings = {
    connectionString : string
    useSqliteForTesting: bool
}

[<CLIMutable>]
type WebServerSettings = {
    enable : bool
    enableDevelopmentMode : bool
    webRoot : string
}

[<CLIMutable>]
type ApiSettings = {
    apiAddress : string
    allowedOrigins : string
    cookieDomain : string
    cookieName : string
    enableSwagger: bool
}

[<CLIMutable>]
type LogLevelSettings = {
    microsoft: LogEventLevel
    aspnetcore: LogEventLevel
    efcore: LogEventLevel
}

[<CLIMutable>]
type LogSettings = {
    directory : string
    levels: LogLevelSettings
}

[<CLIMutable>]
type AppSettings = {
    sql : SqlSettings
    webServer : WebServerSettings
    api : ApiSettings
    log : LogSettings
}

module AppSettings =

    let empty : AppSettings = {
        sql = {
            connectionString = ""
            useSqliteForTesting = false
        }
        webServer = {
            enable = false
            enableDevelopmentMode = false
            webRoot = ""        
        }
        api = {
            apiAddress = ""
            allowedOrigins = ""
            cookieDomain = ""
            cookieName = ""
            enableSwagger = true
        }
        log = {
            directory = ""
            levels = {
                microsoft = LogEventLevel.Warning
                aspnetcore = LogEventLevel.Warning
                efcore = LogEventLevel.Warning
            }
        }
    }