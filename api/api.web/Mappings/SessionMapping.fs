namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module SessionMapping =
    
    let toLoginRequest (source : LoginRequestDto) : LoginRequest =
        {
            username = source.username
            password = source.password
        }

    let toSessionDto (source : Session) : SessionDto =
        {
            id = source.id
            token = source.token
            createdOn = source.createdOn
            expiresOn = source.expiresOn
            user = source.user |> toUserDto
        }
