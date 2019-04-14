open System.IO
open System.Text.RegularExpressions

let getFiles relativeDir filenameRegex =
    let dir = sprintf "%s\\..\\%s" __SOURCE_DIRECTORY__ relativeDir
    Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
    |> Seq.filter(fun f -> Regex.IsMatch(f, filenameRegex))

let fixFile path =
    let mutable text = File.ReadAllText path
    text <- Regex.Replace(text, " +\r", fun _ -> "\r")
    File.WriteAllText(path, text)

let fixBatch relativeDir filenameRegex =
    let files = getFiles relativeDir filenameRegex
    for f in files do
        fixFile f

fixBatch "api"      """.+\.fsx?$"""
fixBatch "utils"    """.+\.fsx?$"""
fixBatch "db"       """.+\.sql$"""
fixBatch "web/src"  """.+\.tsx?$"""
fixBatch "web/test" """.+\.tsx?$"""