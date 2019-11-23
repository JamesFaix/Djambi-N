open System
open System.IO
open System.Net
open CsvHelper
open Newtonsoft.Json.Linq

type Issue = {
    id : int
    name : string
    labels : string list
}

type IssueCsvModel = {
    id : int
    name : string
    labels : string
}

let fetchJsonFromServer () =
    // Create request
    let url = "https://api.github.com/repos/jamesFaix/apex/issues?per_page=100"
    let req = HttpWebRequest.Create(url) :?> HttpWebRequest
    req.Method <- "GET"
    
    // Get response body
    let resp = req.GetResponse()
    use stream = resp.GetResponseStream()
    use reader = new StreamReader(stream)
    let json = reader.ReadToEnd()

    json

let fetchJsonFromFile () =
    let path = Path.Combine(Directory.GetCurrentDirectory(), "data.json")
    File.ReadAllText path

let parseIssues (json : string) : Issue list = 
    let arr = JArray.Parse json    
    let issues : Issue list = 
        arr
        |> Seq.map (fun x ->  
            let i : Issue = {
                id = x.["number"].Value<int>()
                name = x.["title"].Value<string>()
                labels = 
                    x.["labels"] 
                    |> Seq.map (fun l -> l.["name"].Value<string>()) 
                    |> Seq.toList
            }
            i
        )
        |> Seq.toList
    issues

let writeCsv (issues : Issue seq) (path : string) =
    let issues = issues |> Seq.map(fun i -> {
        id = i.id
        name = i.name
        labels = String.Join(", ", i.labels)
    })

    use writer = new StreamWriter(path)
    use csv = new CsvWriter(writer)
    csv.WriteRecords issues
    ()

[<EntryPoint>]
let main _ =
    let json = fetchJsonFromFile()
    let issues = parseIssues json
    let filePath = Path.Combine(Directory.GetCurrentDirectory(), "issues.csv")
    writeCsv issues filePath

    printf "Done"
    Console.Read() |> ignore
    0 // return an integer exit code
