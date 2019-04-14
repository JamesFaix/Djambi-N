#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.JavaScript.Npm"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open System.Diagnostics
open System.IO

//Target names
let defaultTarget = "default"
let all = "all"

let checkForEnv = "check-env"

let paketRestore = "paket-restore"
let buildApi = "build-api"
let buildWeb = "build-web"
let dbReset = "db-reset"
let genClient = "gen-client"
let restoreWeb = "restore-web"
let buildAll = "build-all"

let runApi = "run-api"
let runWeb = "run-web"
let runAll = "run-all"

let fsLint = "fs-lint"
let lintAll = "lint-all"

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
    DotNet.exec id "build" path |> ignore
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

Target.create checkForEnv (fun _ ->
    let path = sprintf "%s/environment.json" __SOURCE_DIRECTORY__
    if not (File.Exists path)
    then failwith "environment.json file not found. A template can be found in the wiki."
)

Target.create paketRestore (fun _ ->
    Process.Start(".paket/paket.exe", "restore")
    |> ignore
)
Target.create buildApi (dotnetBuild "api/api.host/api.host.fsproj")
Target.create dbReset (dotNetRun "utils/db-reset/db-reset.fsproj")
Target.create genClient (dotNetRun "utils/client-generator/client-generator.fsproj")
Target.create restoreWeb (fun _ -> Npm.install setNpmParams)
Target.create buildWeb (fun _ -> Npm.run "build" setNpmParams)

Target.create fsLint (fun _ ->
    let projects = !! "**/*.fsproj"
    for p in projects do
        let args = sprintf """-f "%s" """ p
        DotNet.exec id "fsharplint" args
        |> ignore
)

Target.create testApiUnit (dotNetTest "api/tests/api.unitTests/api.unitTests.fsproj")
Target.create testApiInt (dotNetTest "api/tests/api.integrationTests/api.integrationTests.fsproj")
Target.create testWebUnit (fun _ -> Npm.run "test" setNpmParams)

Target.create runApi (launchConsole "api/api.host" "dotnet" ["run api.host.fsproj"])
Target.create runWeb (launchConsole "web" "http-server" [])

Target.create buildAll ignore
Target.create lintAll ignore
Target.create testAll ignore
Target.create runAll ignore
Target.create all ignore

//Dependencies

open Fake.Core.TargetOperators

checkForEnv ==> dbReset
checkForEnv ==> buildApi
checkForEnv ==> genClient

paketRestore ?=> dbReset
paketRestore ?=> buildApi
paketRestore ?=> genClient

dbReset ?=> buildApi
buildApi ?=> genClient
genClient ?=> buildWeb
buildApi ?=> runApi
restoreWeb ?=> buildWeb
buildWeb ?=> runWeb

buildApi ?=> testApiUnit
buildApi ?=> testApiInt
genClient ?=> testApiUnit
genClient ?=> testApiInt
buildWeb ?=> testWebUnit

testApiUnit ?=> runApi
testApiInt ?=> runApi
testWebUnit ?=> runWeb

buildAll <==
    [
        paketRestore
        dbReset
        buildApi
        genClient
        restoreWeb
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

lintAll <==
    [
        fsLint
    ]

all <==
    [
        buildAll
        runAll
        testAll
        lintAll
    ]

//Start
Target.runOrDefault defaultTarget