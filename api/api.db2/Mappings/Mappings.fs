module Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System.ComponentModel

let toPrivilege (source : PrivilegeSqlModel) : Privilege =
    match source.Id with
    | 1uy -> Privilege.EditUsers
    | 2uy -> Privilege.EditPendingGames
    | 3uy -> Privilege.OpenParticipation
    | 4uy -> Privilege.Snapshots
    | 5uy -> Privilege.ViewGames
    | _ -> raise <| InvalidEnumArgumentException()

let toUserDetails (source : UserSqlModel) : UserDetails =
    {
        id = source.Id
        name = source.Name
        password = source.Password
        failedLoginAttempts = int source.FailedLoginAttempts
        lastFailedLoginAttemptOn = source.LastFailedLoginAttemptOn |> Option.ofNullable
        privileges = source.Privileges |> Seq.map toPrivilege |> Seq.toList
    }    