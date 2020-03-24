namespace Apex.Api.Web.Model

open System

type PrivilegeDto =
    | EditUsers = 1
    | EditPendingGames = 2
    | OpenParticipation = 3
    | ViewGames = 4
    | Snapshots = 5

type UserDto = {
    id : int
    name : string
    privileges : List<PrivilegeDto>
}

[<CLIMutable>]
type CreateUserRequestDto = {
    name : string
    password : string
}

type CreationSourceDto = {
    userId : int
    userName : string
    time : DateTime
}