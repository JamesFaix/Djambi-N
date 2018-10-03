## Required Tools
- .NET Core 2.1 SDK
- SQL Server
- NPM
- Webpack + plugins
    `npm install --save-dev webpack`
    `npm install --save-dev typescript ts-loader`
    `npm install --save-dev style-loader`
- Web server
    `npm install http-server -g`    

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
- View `localhost:8080` in browser