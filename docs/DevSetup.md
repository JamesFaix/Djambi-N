## Required Tools
- .NET Core 2.1 SDK
- SQL Server
- NPM
- Webpack + plugins
    `npm install --save-dev webpack`
    `npm install --save-dev typescript ts-loader`
    `npm install --save-dev style-loader`
    

## Environment configuration
- At the repository root, create an `environment.json` file for some global configuration. This file is not checked into source control, but is required.

```
{
    "sqlAddress": "localhost"
}
```
`sqlAddress` should be the address or alias for your SQL Server instance. Possibly `localhost` or `localhost\\SQLExpress` 
