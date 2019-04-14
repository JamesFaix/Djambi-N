open System.IO
open System.Text.RegularExpressions

let rootPath = sprintf "%s\\.." __SOURCE_DIRECTORY__
let filePattern = "*.fs" //CAREFUL WHAT YOU WISH FOR
let filePaths = Directory.EnumerateFiles(rootPath, filePattern, SearchOption.AllDirectories)

for f in filePaths do
    let mutable text = File.ReadAllText f
    text <- Regex.Replace(text, " +\r", fun m -> "\r")
    File.WriteAllText(f, text)