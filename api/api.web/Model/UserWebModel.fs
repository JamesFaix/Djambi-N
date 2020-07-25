namespace Apex.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations
open Apex.Api.Enums

type UserDto = {
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
    userId : int

    [<Required>]
    userName : string

    time : DateTime
}