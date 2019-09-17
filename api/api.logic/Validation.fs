module Djambi.Api.Logic.Validation

open System.Text.RegularExpressions

let private descriptionRegex = Regex(""".{0,100}""")
let private nameRegex = Regex("""[A-Za-z0-9_-]{1,20}""")
let private passwordRegex = Regex("""[A-Za-z0-9_-]{6,20}""")

let isValidSnapshotDescription(desc : string) : bool =
    descriptionRegex.IsMatch desc  

let isValidGameDescription(desc : string) : bool =
    descriptionRegex.IsMatch desc

let isValidPlayerName(name : string) : bool =
    nameRegex.IsMatch name

let isValidUserName(name : string) : bool =
    nameRegex.IsMatch name

let isValidPassord(password : string) : bool =
    passwordRegex.IsMatch password