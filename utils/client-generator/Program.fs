module Djambi.ClientGenerator.Program

open System.IO
open System.Reflection
open Microsoft.Extensions.Configuration
open Djambi.ClientGenerator.Annotations
open Djambi.Utilities

let renderModel (renderers : IRenderer list, config : IConfigurationRoot, rootPath : string) : Unit =
    printfn "Loading model assembly..."
    let assembly = typeof<Djambi.Api.Model.BoardModel.Board>.Assembly

    let types = 
        assembly.GetTypes()
        |> Seq.filter (fun t -> t.GetCustomAttribute<ClientTypeAttribute>() <> null)
        |> Seq.toList

    for r in renderers do
        printfn "Rendering %s model..." r.name
        let fullText = r.renderModel types
        let outputPath = Path.Combine(rootPath, config.[r.modelOutputPathSetting])
        File.WriteAllText(outputPath, fullText)

let renderFunctions (renderers : IRenderer list, config : IConfigurationRoot, rootPath : string) : Unit =
    printfn "Loading functions assembly..."
    let assembly = typeof<Djambi.Api.Logic.Marker>.Assembly
    
    let methods =
        assembly.GetTypes()
        |> Seq.collect (fun t -> t.GetMethods())
        |> Seq.filter (fun m -> m.GetCustomAttribute<ClientFunctionAttribute>() <> null)
        |> Seq.toList

    for r in renderers do
        printfn "Rendering %s functions..." r.name
        let fullText = r.renderFunctions methods
        let outputPath = Path.Combine(rootPath, config.[r.endpointsOutputPathSetting])
        File.WriteAllText(outputPath, fullText)

[<EntryPoint>]
let main argv =    
   
    printfn "Djambi API Client Generator"
    printfn "---------------------------"

    let depthFromRoot = 5
    
    let root = Environment.rootDirectory(depthFromRoot)
    let config = ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile(Environment.environmentConfigPath(depthFromRoot), false)
                    .Build()
    
    let renderers = 
        [
            TypeScriptRenderer() :> IRenderer
        ]

    renderModel (renderers, config, root)
    renderFunctions (renderers, config, root)

    printfn "Done"
    
    0 // return an integer exit code