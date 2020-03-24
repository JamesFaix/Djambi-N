namespace Apex.Api.Web.Model

open System

type SessionDto = {
    id : int
    user : UserDto
    token : string
    createdOn : DateTime
    expiresOn : DateTime
}

[<CLIMutable>]
type LoginRequestDto = {
    username : string
    password : string
}