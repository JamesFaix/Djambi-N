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
        |> Seq.map (fun t -> (t, t.GetCustomAttribute<ClientTypeAttribute>()))
        |> Seq.filter (fun (_, attr) -> attr <> null)
        |> Seq.sortBy (fun (t, attr) -> attr.section, t.Name)
        |> Seq.map (fun (t, _) -> t)
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
        |> Seq.map (fun m -> (m, m.GetCustomAttribute<ClientFunctionAttribute>()))
        |> Seq.filter (fun (_, attr) -> attr <> null)
        |> Seq.sortBy (fun (m, attr) -> attr.section, m.Name)
        |> Seq.map (fun (m, _) -> m)
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
    
    let env = Environment.load(depthFromRoot);
    let config = ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .Build()
    
    let renderers = 
        [
            TypeScriptRenderer() :> IRenderer
        ]

    renderModel (renderers, config, env.root)
    renderFunctions (renderers, config, env.root)

    printfn "Done"
    
    0 // return an integer exit code