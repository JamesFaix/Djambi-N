module Djambi.ClientGenerator.Program

open System
open System.IO
open System.Linq
open System.Reflection
open Microsoft.Extensions.Configuration
open Djambi.ClientGenerator.Annotations
open Djambi.Utilities

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
                    
    let modelAssemblyPath = Path.Combine(root, config.["ModelAssemblyPath"])
    let typeScriptModelOutputPath = Path.Combine(root, config.["TypeScriptModelOutputPath"])
    
    printfn "Loading types..."

    let assembly = Assembly.LoadFile modelAssemblyPath
    
    let types = 
        assembly.GetTypes() 
        //Filter out types not marked with ClientTypeAttribute
        |> Seq.filter (fun t -> 
            t.GetCustomAttributes() 
            |> Enumerable.OfType<ClientTypeAttribute> 
            |> (not << Seq.isEmpty) 
        )
        |> Seq.toList
    
    printfn "Rendering TypeScript client..."

    let renderer = TypeScriptRenderer() :> IRenderer
    let fullText = renderer.renderTypes types
    File.WriteAllText(typeScriptModelOutputPath, fullText)

    printfn "Done"

    Console.Read() |> ignore

    0 // return an integer exit code