#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild
nuget Fake.IO.FileSystem
nuget Fake.JavaScript.Npm"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.JavaScript
open System.Diagnostics

//Target names
let defaultTarget = "default"
let all = "all"
let buildApi = "build-api"
let buildWeb = "build-web"
let dbReset = "db-reset"
let genClient = "gen-client"
let runApi = "run-api"
let runWeb = "run-web"
let testApiInt = "test-api-int"
let testApiUnit = "test-api-unit"
let testWebUnit = "test-web-unit"

//Targets
Target.create defaultTarget (fun _ ->
    Trace.trace "Enter a target, or use 'fake build --list' to see all targets."
)

Target.create all ignore

let getDir (subDir : string) = sprintf "%s/%s" __SOURCE_DIRECTORY__ subDir

let buildFsProj (path : string) (_ : TargetParameter) =
    DotNet.exec id "build" path |> ignore

let runFsProj (path : string) (_ : TargetParameter) =
    DotNet.exec id "run" ("--project " + path) |> ignore

let launchConsole (dir : string) (command : string) (args : string list) (_ : TargetParameter) =
    let psi = ProcessStartInfo()
    psi.FileName <- command
    psi.Arguments <- System.String.Join(" ", args)
    psi.WorkingDirectory <- getDir dir
    psi.UseShellExecute <- true
    Process.Start psi |> ignore

Target.create buildApi (buildFsProj "api/api.host/api.host.fsproj")
Target.create dbReset (runFsProj "utils/db-reset/db-reset.fsproj")
Target.create genClient (runFsProj "utils/client-generator/client-generator.fsproj")
Target.create buildWeb (fun _ ->
    let setParams (o : Npm.NpmParams) = { o with WorkingDirectory = getDir "web" }
    Npm.install setParams
    Npm.run "build" setParams
)
Target.create runApi (launchConsole "api/api.host" "dotnet" ["run api.host.fsproj"])
Target.create runWeb (launchConsole "web" "http-server" [])

//Dependencies

open Fake.Core.TargetOperators

dbReset ?=> buildApi
buildApi ?=> genClient
genClient ?=> buildWeb
buildApi ?=> runApi
buildWeb ?=> runWeb

all <==
    [
        dbReset
        buildApi
        genClient
        buildWeb
        runApi
        runWeb
    ]

//Start
Target.runOrDefault defaultTarget