module Djambi.Utilities.Environment

open System
open System.IO
open System.Linq
open System.Reflection

let rootDirectory (currentAssemblyDepth : int) : string = 
    let asm = Assembly.GetExecutingAssembly()
    let asmDir = Uri(asm.CodeBase).LocalPath |> Path.GetDirectoryName 
    let movesUp = Enumerable.Repeat("..\\", currentAssemblyDepth) |> Seq.toArray
    let relativeConfigPath = String.Join("", movesUp)
    let fullRelativePath = Path.Combine(asmDir, relativeConfigPath)
    Path.GetFullPath(Uri(fullRelativePath).LocalPath)

let environmentConfigPath (currentAssemblyDepth : int) : string = 
    rootDirectory(currentAssemblyDepth) + "environment.json"