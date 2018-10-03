## Required Tools
- .NET Core 2.1 SDK
- XUnit test runner
- SQL Server Express or Developer Edition
- NPM
- Webpack + plugins
    `npm install --save-dev webpack`
    `npm install --save-dev typescript ts-loader`
    `npm install --save-dev style-loader`
- Web server
    `npm install http-server -g`    

## SQL Server configuration
- See https://blogs.msdn.microsoft.com/walzenbach/2010/04/14/how-to-enable-remote-connections-in-sql-server-2008/
- In SSMS, right click server in Object Explorer, select _Properties_ > _Connections_, make sure _Allow remote connections to this server_ is checked.
- In Sql Server Configuration Manager 
    - In _SQL Server Network Configuration_ > _Protocols for SQLSERVER_, make sure _TCP/IP_ is enabled.
    - Right click _TCP/IP_ > Select _Properties_, make sure _TCP Port_ is set to _1433_.
- Add inbound Windows Firewall rule to allow TCP on port 1433 within the Domain.
- Restart SQL service.

## Environment configuration
- At the repository root, create an `environment.json` file for some global configuration. This file is not checked into source control, but is required.

```
{
  "sqlAddress": "localhost",              //Address of SQL instance
  "apiAddress": "http://localhost:5100",  //Address used for API
  "webAddress": "http://localhost:8080"   //Address used for web server
}
```

## Start up
- Build .NET solution `Djambi.sln`
- Run `Djambi.Utilities.DatabaseReset.exe`, this will build the SQL schema
- Start `Djambi.Api`
    - Currently the web client expects it should be on port `54835`. When running from Visual Studio through IIS Express, this is the default.
- Build web client 
    - `cd web & npm run build`
    - This is the default build task in VS Code
- Start web server
    - From root `cd web/dist & http-server`
    - Port `8080` is the default for `http-server`, but it can be changed with the argument `-p 1234`
- View `localhost:8080` in browser