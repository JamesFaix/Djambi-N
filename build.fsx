open System.Diagnostics
#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild
nuget Fake.IO.FileSystem"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
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

let buildFsProject (dir : string) (name : string) =
    (fun (_ : TargetParameter) ->
        let path = sprintf "./%s/%s/%s.fsproj" dir name name
        DotNet.exec id "build" path |> ignore
    )

let runFsProject (dir : string) (name : string) (newWindow : bool)=
    (fun (_ : TargetParameter) ->
        let path = sprintf "./%s/%s/%s.fsproj" dir name name

        if not newWindow
        then DotNet.exec id "run" ("--project " + path)
             |> ignore
        else
            let psi = ProcessStartInfo()
            psi.FileName <- "dotnet"
            psi.Arguments <- sprintf "run %s.fsproj" name
            psi.WorkingDirectory <- sprintf "%s/%s/%s" __SOURCE_DIRECTORY__ dir name
            psi.CreateNoWindow <- false
            psi.UseShellExecute <- false
            Process.Start psi |> ignore
    )

Target.create buildApi (buildFsProject "api" "api.host")
Target.create dbReset (runFsProject "utils" "db-reset" false)
Target.create genClient (runFsProject "utils" "client-generator" false)
Target.create runApi (runFsProject "api" "api.host" true)

//Dependencies

open Fake.Core.TargetOperators

dbReset ?=> buildApi
buildApi ?=> genClient
runApi ==> buildApi

all <==
    [
        dbReset
        buildApi
        genClient
    ]

//Start
Target.runOrDefault defaultTarget