## Required Tools
- .NET Core 2.1 SDK
- XUnit test runner
- SQL Server Express or Developer Edition
- NPM
- TypeScript Compiler
- F# Compiler
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
  "sqlAddress": "localhost",               //Address of SQL instance
  "apiAddress": "http://localhost:5100",   //Address used for API
  "webAddress": "http://localhost:8080",   //Address used for web server
  "adminUsername": "admin",                //Username for primary admin user
  "adminPassword": "admin"                 //Password for primary admin user
}
```

## Build script
- Build script written in Cake
- Powershell requires allowing scripts
    - Run Powershell as admin
    - `set-executionpolicy unrestricted`
- Run the `build.ps1` or `build.sh` file from the repository root
    - Initial setup is `./build.ps1 -target full`
    - See `build.cake` for other targets
