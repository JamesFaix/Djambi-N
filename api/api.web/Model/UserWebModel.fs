namespace Apex.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations

type PrivilegeDto =
    | EditUsers = 1
    | EditPendingGames = 2
    | OpenParticipation = 3
    | ViewGames = 4
    | Snapshots = 5

type UserDto = {
    id : int

    [<Required>]
    name : string

    [<Required>]
    privileges : List<PrivilegeDto>
}

[<CLIMutable>]
type CreateUserRequestDto = {
    [<Required>]
    [<RegularExpression("[a-zA-Z0-9\-_]+")>]
    name : string

    [<Required>]
    [<StringLength(20, MinimumLength = 6)>]
    password : string
}

type CreationSourceDto = {
    userId : int

    [<Required>]
    userName : string

    time : DateTime
}