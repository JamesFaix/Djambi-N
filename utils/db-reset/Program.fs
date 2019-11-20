open System
open System.Data.SqlClient
open System.IO
open System.Text.RegularExpressions
open Dapper
open Apex.Utilities.DbReset

let options = Config.options

let private executeCommand (cnStr : string)(command : string) : unit =
    use cn = new SqlConnection(cnStr)

    cn.Execute(command) |> ignore

let private dropAndCreateDb() : unit =
    printfn "Dropping and creating database"
    let sql = "IF EXISTS(SELECT * FROM sys.databases WHERE name='Apex')
               DROP DATABASE Apex;
               CREATE DATABASE Apex;
               ALTER DATABASE Apex SET COMPATIBILITY_LEVEL = 120" //120 = SQL Server 2014
    executeCommand options.masterConnectionString sql

let private loadFile (relativePath : string) : unit =
    printfn "Loading %s" relativePath
    let absolutePath = Path.Combine(options.sqlRoot, relativePath)
    let text = File.ReadAllText(absolutePath)
    let commands = Regex.Split(text, "\s+GO")
                   |> Seq.filter (String.IsNullOrEmpty >> not)
    for c in commands do
        executeCommand options.apexConnectionString c

let getFilesInOrder : string seq =
    seq {
        //Order is very important for foreign keys
        let tables = [

            //Static data first
            "EventKinds"
            "GameStatuses"
            "NeutralPlayerNames"
            "PlayerKinds"
            "PlayerStatuses"
            "Privileges"

            //Then entities
            "Users"
            "Sessions"
            "Games"
            "Players"
            "Events"
            "Snapshots"

            //Then relations
            "UserPrivileges"
        ]

        yield! tables |> Seq.map (fun name -> Path.Combine("Tables", sprintf "dbo.%s.sql" name))

        let views = [
            "VGamePlayerCounts"
            "VGameUsers"
            "VLatestEvents"
            "VUserViewableGames"
        ]

        yield! views |> Seq.map (fun name -> Path.Combine("Views", sprintf "%s.sql" name))

        let getFiles folder =
            Path.Combine(options.sqlRoot, folder)
            |> Directory.EnumerateFiles
            |> Seq.map (fun path -> Path.Combine(folder, Path.GetFileName(path)))

        let folders = [
            Path.Combine("Types", "User-defined Data Types")
            "Stored Procedures"
            "Data"
        ]

        yield! folders |> Seq.collect getFiles
    }

[<EntryPoint>]
let main _ =
    dropAndCreateDb()

    for f in getFilesInOrder do
        loadFile f

    printfn "Done"
    0
