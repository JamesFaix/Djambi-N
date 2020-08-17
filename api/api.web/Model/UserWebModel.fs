namespace Djambi.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations
open Djambi.Api.Enums

type UserDto = {
    [<Required>]
    id : int

    [<Required>]
    name : string

    [<Required>]
    privileges : List<Privilege>
}

[<CLIMutable>]
type CreateUserRequestDto = {
    [<Required>]
    [<RegularExpression("[a-zA-Z0-9\-_]+")>]
    [<StringLength(20, MinimumLength = 1)>]
    name : string

    [<Required>]
    [<StringLength(20, MinimumLength = 6)>]
    password : string
}

type CreationSourceDto = {
    [<Required>]
    userId : int

    [<Required>]
    userName : string

    [<Required>]
    time : DateTime
}