module Djambi.ClientGenerator.Program

open System
open System.IO
open System.Reflection
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
    let types = assembly.GetTypes() |> Seq.toList
    let fullText = renderer.renderTypes types

    File.WriteAllText(typeScriptModelOutputPath, fullText)
    Console.Write fullText

    Console.Read() |> ignore

    0 // return an integer exit code