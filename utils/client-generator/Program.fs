module Djambi.ClientGenerator.Program

open System
open System.IO
open System.Reflection
open System.Text
open Microsoft.Extensions.Configuration
open Djambi.Utilities

[<EntryPoint>]
let main argv =    
   
    let depthFromRoot = 5
    
    let root = Environment.rootDirectory(depthFromRoot)
    let config = ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile(Environment.environmentConfigPath(depthFromRoot), false)
                    .Build()
                    
    let renderer = TypeScriptRenderer() :> IRenderer

    let modelAssemblyPath = Path.Combine(root, config.["ModelAssemblyPath"])
    let typeScriptModelOutputPath = Path.Combine(root, config.["TypeScriptModelOutputPath"])
    
    let assembly = Assembly.LoadFile modelAssemblyPath
    
    let typesAndKinds = 
        assembly.GetTypes()
        |> Seq.map(fun t -> 
            let kind = TypeKind.fromType t
            (t, kind)
        )
        |> Seq.filter (fun (_, kind) -> kind <> TypeKind.Unsupported)

    let sb = StringBuilder()

    for (t, _) in typesAndKinds do
        let text = renderer.renderType t
        sb.AppendLine text |> ignore
    
    let fullText = sb.ToString()

    File.WriteAllText(typeScriptModelOutputPath, fullText)
    Console.Write fullText

    Console.Read() |> ignore

    0 // return an integer exit code