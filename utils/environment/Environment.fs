namespace Djambi.Utilities

type Environment =
    {
        root : string
        sqlAddress : string
        apiAddress : string
        cookieDomain : string
        webAddress : string
        adminUsername : string
        adminPassword : string
    }

module Environment =

    open System
    open System.IO
    open System.Linq
    open System.Reflection
    open Microsoft.Extensions.Configuration

    let private rootDirectory (currentAssemblyDepth : int) : string =
        let asm = Assembly.GetExecutingAssembly()
        let asmDir = Uri(asm.CodeBase).LocalPath |> Path.GetDirectoryName
        let movesUp = Enumerable.Repeat("..\\", currentAssemblyDepth) |> Seq.toArray
        let relativeConfigPath = String.Join("", movesUp)
        let fullRelativePath = Path.Combine(asmDir, relativeConfigPath)
        Path.GetFullPath(Uri(fullRelativePath).LocalPath)

    let environmentConfigPath (currentAssemblyDepth : int) : string =
        rootDirectory(currentAssemblyDepth) + "environment.json"

    let load(currentAssemblyDepth : int) =
        let configRoot =
            ConfigurationBuilder()
                .AddJsonFile(environmentConfigPath(currentAssemblyDepth), false)
                .Build()
        {
            root = rootDirectory(currentAssemblyDepth)
            sqlAddress = configRoot.["sqlAdderss"]
            apiAddress = configRoot.["apiAddress"]
            cookieDomain = configRoot.["cookieDomain"]
            webAddress = configRoot.["webAddress"]
            adminUsername = configRoot.["adminUsername"]
            adminPassword = configRoot.["adminPassword"]
        }