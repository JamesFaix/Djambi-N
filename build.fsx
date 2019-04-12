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
let restoreWeb = "restore-web"
let buildAll = "build-all"

let runApi = "run-api"
let runWeb = "run-web"
let runAll = "run-all"

let testApiInt = "test-api-int"
let testApiUnit = "test-api-unit"
let testWebUnit = "test-web-unit"
let testAll = "test-all"

//Targets
Target.create defaultTarget (fun _ ->
    Trace.trace "Enter a target, or use 'fake build --list' to see all targets."
)

let getDir (subDir : string) = sprintf "%s/%s" __SOURCE_DIRECTORY__ subDir

let dotnetBuild (path : string) (_ : TargetParameter) =
    DotNet.exec id "build" path |> ignore

let dotNetRun (path : string) (_ : TargetParameter) =
    DotNet.exec id "run" ("--project " + path) |> ignore

let dotNetTest (path : string) (_ : TargetParameter) =
    DotNet.test
        (fun o ->
            let common = { o.Common with Verbosity = Some DotNet.Verbosity.Normal }
            { o with
                Common = common
                NoBuild = true
            }
        )
        path
    |> ignore

let setNpmParams (o : Npm.NpmParams) = { o with WorkingDirectory = getDir "web" }

let launchConsole (dir : string) (command : string) (args : string list) (_ : TargetParameter) =
    let psi = ProcessStartInfo()
    psi.FileName <- command
    psi.Arguments <- System.String.Join(" ", args)
    psi.WorkingDirectory <- getDir dir
    psi.UseShellExecute <- true
    Process.Start psi |> ignore

Target.create buildApi (dotnetBuild "api/api.host/api.host.fsproj")
Target.create dbReset (dotNetRun "utils/db-reset/db-reset.fsproj")
Target.create genClient (dotNetRun "utils/client-generator/client-generator.fsproj")
Target.create restoreWeb (fun _ -> Npm.install setNpmParams)
Target.create buildWeb (fun _ -> Npm.run "build" setNpmParams)
Target.create buildAll ignore

Target.create testApiUnit (dotNetTest "api/tests/api.unitTests/api.unitTests.fsproj")
Target.create testApiInt (dotNetTest "api/tests/api.integrationTests/api.integrationTests.fsproj")
Target.create testWebUnit (fun _ -> Npm.run "test" setNpmParams)
Target.create testAll ignore

Target.create runApi (launchConsole "api/api.host" "dotnet" ["run api.host.fsproj"])
Target.create runWeb (launchConsole "web" "http-server" [])
Target.create runAll ignore

Target.create all ignore

//Dependencies

open Fake.Core.TargetOperators

dbReset ?=> buildApi
buildApi ?=> genClient
genClient ?=> buildWeb
buildApi ?=> runApi
buildWeb ?=> runWeb
buildApi ?=> testApiUnit
buildApi ?=> testApiInt
buildWeb ?=> testWebUnit

buildAll <==
    [
        dbReset
        buildApi
        genClient
        buildWeb
    ]

runAll <==
    [
        runApi
        runWeb
    ]

testAll <==
    [
        testApiUnit
        testApiInt
        testWebUnit
    ]

all <==
    [
        buildAll
        runAll
        testAll
    ]

//Start
Target.runOrDefault defaultTarget