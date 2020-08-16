namespace Djambi.Api.Db.Mappings

open Djambi.Api.Db.Model
open Djambi.Api.Model
open System.ComponentModel
open Djambi.Api.Enums

type ArrayList<'a> = System.Collections.Generic.List<'a>

[<AutoOpen>]
module UserMappings =

    let mapUserPrivileges (source : seq<UserPrivilegeSqlModel>) : List<Privilege> =
        if source = null
        then []
        else
            source
            |> Seq.map (fun up -> up.PrivilegeId) 
            |> Seq.toList


    let toUserDetails (source : UserSqlModel) : UserDetails =
        {
            id = source.UserId
            name = source.Name
            password = source.Password
            failedLoginAttempts = int source.FailedLoginAttempts
            lastFailedLoginAttemptOn = source.LastFailedLoginAttemptOn |> Option.ofNullable
            privileges = source.UserPrivileges |> mapUserPrivileges
        }    

    let toUser (source : UserSqlModel) : User =
        {
            id = source.UserId
            name = source.Name
            privileges = source.UserPrivileges |> mapUserPrivileges
        } 