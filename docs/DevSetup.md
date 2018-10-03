## Required Tools
- .NET Core 2.1 SDK
- SQL Server

## Environment configuration
- At the repository root, create an `environment.json` file for some global configuration.

```
{
    "sqlAddress": "localhost"
}
```
`sqlAddress` should be the address or alias for your SQL Server instance. Possibly `localhost` or `localhost\\SQLExpress` 
