namespace Apex.Api.Model.Configuration

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
type LogSettings = {
    directory : string
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
        }
    }