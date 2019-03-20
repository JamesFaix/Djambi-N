# Developer Setup / Architecture Notes

## Web Client
- Written using TypeScript and React
- Scripts require NPM and Webpack
- Using `http-server` to host in development
    - `npm install http-server -g`

## API
- Self-hosting .NET Core application
- Written in F# using Giraffe framework
- Unit and Integration tests require XUnit runner, Contract tests require NUnit
- Requires .NET Core 2.1 SDK

## Database
- SQL Server Express or Developer Edition.
- SQL code sticks to ANSI constructs as much as possible, but in some cases T-SQL constructs are used. This should be avoided so vendors can be changed if needed later.
- TSQL compatabilty level is set to T-SQL 2014.

## Environment configuration
- At the repository root, create an `environment.json` file for some global configuration. This file is not checked into source control, but is required. Each application uses this file to communicate with the other applications.

```
{
  "sqlAddress": "localhost",               //Address of SQL instance
  "apiAddress": "http://localhost:5100",   //Address used for API
  "cookieDomain": "localhost",             //Domain to use for cookies
  "webAddress": "http://localhost:8080",   //Address used for web server
  "adminUsername": "admin",                //Username for primary admin user used for contract tests
  "adminPassword": "admin"                 //Password for primary admin user used for contract tests
}
```

## Build script
- Script written in Cake
- Powershell requires allowing scripts
    - Run Powershell as admin
    - `set-executionpolicy unrestricted`
- Run the `build.ps1` or `build.sh` file from the repository root
    - Run `./build.ps1 -target help` for further documentation

## SQL Server configuration
- See https://blogs.msdn.microsoft.com/walzenbach/2010/04/14/how-to-enable-remote-connections-in-sql-server-2008/
- In SSMS, right click server in Object Explorer, select _Properties_ > _Connections_, make sure _Allow remote connections to this server_ is checked.
- In Sql Server Configuration Manager 
    - In _SQL Server Network Configuration_ > _Protocols for SQLSERVER_, make sure _TCP/IP_ is enabled.
    - Right click _TCP/IP_ > Select _Properties_, make sure _TCP Port_ is set to _1433_.
- Add inbound Windows Firewall rule to allow TCP on port 1433 within the Domain.
- Restart SQL service.

## Recommended IDE
- Visual Studio 2017
- Workloads
  - ASP.NET and web development
  - .NET Core cross-platform development
- Components
  - SQL Server Data Tools
  - F# language support
  - F# langauge support for web projects
- Extensions
  - `Whack Whack Terminal` to add IDE terminal window for running build scripts
  - `Markdown Editor` for updating documentation