namespace Apex.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations

type SessionDto = {
    id : int

    [<Required>]
    user : UserDto

    [<Required>]
    token : string

    createdOn : DateTime

    expiresOn : DateTime
}

[<CLIMutable>]
type LoginRequestDto = {
    [<Required>]
    username : string

    [<Required>]
    [<StringLength(20, MinimumLength = 6)>]
    password : string
}