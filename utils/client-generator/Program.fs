module Apex.ClientGenerator.Program

open System.IO
open System.Reflection
open Microsoft.Extensions.Configuration
open Apex.ClientGenerator.Annotations

let renderModel (renderers : IRenderer list, config : IConfigurationRoot) : Unit =
    printfn "Loading model assembly..."
    let assembly = typeof<Apex.Api.Model.BoardModel.Board>.Assembly

    let types =
        assembly.GetTypes()
        |> Seq.map (fun t -> (t, t.GetCustomAttribute<ClientTypeAttribute>()))
        |> Seq.filter (fun (_, attr) -> not (isNull attr))
        |> Seq.sortBy (fun (t, attr) -> attr.section, t.Name)
        |> Seq.map (fun (t, _) -> t)
        |> Seq.toList

    for r in renderers do
        printfn "Rendering %s model..." r.name
        let fullText = r.renderModel types
        let outputPath = Path.Combine(config.["repositoryRoot"], config.[r.modelOutputPathSetting])
        File.WriteAllText(outputPath, fullText)

let renderFunctions (renderers : IRenderer list, config : IConfigurationRoot) : Unit =
    printfn "Loading functions assembly..."
    let assembly = typeof<Apex.Api.Logic.Interfaces.IBoardManager>.Assembly

    let methods =
        assembly.GetTypes()
        |> Seq.collect (fun t -> t.GetMethods())
        |> Seq.map (fun m -> (m, m.GetCustomAttribute<ClientFunctionAttribute>()))
        |> Seq.filter (fun (_, attr) -> not (isNull attr))
        |> Seq.sortBy (fun (m, attr) -> attr.section, m.Name)
        |> Seq.map (fun (m, _) -> m)
        |> Seq.toList

    for r in renderers do
        printfn "Rendering %s functions..." r.name
        let fullText = r.renderFunctions methods
        let outputPath = Path.Combine(config.["repositoryRoot"], config.[r.endpointsOutputPathSetting])
        File.WriteAllText(outputPath, fullText)

[<EntryPoint>]
let main argv =

    printfn "Apex API Client Generator"
    printfn "---------------------------"

    let config = 
        ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables("APEX_")
            .Build()

    let renderers =
        [
            TypeScriptRenderer() :> IRenderer
        ]

    renderModel (renderers, config)
    renderFunctions (renderers, config)

    printfn "Done"

    0 // return an integer exit code