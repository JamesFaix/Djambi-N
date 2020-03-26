namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System.ComponentModel

[<AutoOpen>]
module UserMappings =

    let toPrivilege (source : byte) : Privilege =
        match source with
        | 1uy -> Privilege.EditUsers
        | 2uy -> Privilege.EditPendingGames
        | 3uy -> Privilege.OpenParticipation
        | 4uy -> Privilege.Snapshots
        | 5uy -> Privilege.ViewGames
        | _ -> raise <| InvalidEnumArgumentException()
    
    let toPrivilegeSqlId (source : Privilege) : byte =
        match source with
        | Privilege.EditUsers -> 1uy
        | Privilege.EditPendingGames -> 2uy
        | Privilege.OpenParticipation -> 3uy
        | Privilege.Snapshots -> 4uy
        | Privilege.ViewGames -> 5uy

    let toUserDetails (source : UserSqlModel) : UserDetails =
        {
            id = source.Id
            name = source.Name
            password = source.Password
            failedLoginAttempts = int source.FailedLoginAttempts
            lastFailedLoginAttemptOn = source.LastFailedLoginAttemptOn |> Option.ofNullable
            privileges = source.UserPrivileges |> Seq.map (fun up -> up.PrivilegeId |> toPrivilege) |> Seq.toList
        }    

    let toUser (source : UserSqlModel) : User =
        {
            id = source.Id
            name = source.Name
            privileges = source.UserPrivileges |> Seq.map (fun up -> up.PrivilegeId |> toPrivilege) |> Seq.toList
        } 