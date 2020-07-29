namespace Apex.Api.Model.Configuration

open Serilog.Events

[<CLIMutable>]
type SqlSettings = {
    connectionString : string
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
    webAddress : string
    cookieDomain : string
    cookieName : string
}

[<CLIMutable>]
type LogLevelSettings = {
    microsoft: LogEventLevel
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
        }
        webServer = {
            enable = false
            enableDevelopmentMode = false
            webRoot = ""        
        }
        api = {
            apiAddress = ""
            webAddress = ""
            cookieDomain = ""
            cookieName = ""
        }
        log = {
            directory = ""
            levels = {
                microsoft = LogEventLevel.Warning
            }
        }
    }