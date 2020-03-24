namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module SessionMapping =
    
    let toLoginRequest (source : LoginRequestDto) : LoginRequest =
        {
            username = source.UserName
            password = source.Password
        }

    let toSessionDto (source : Session) : SessionDto =
        let result = SessionDto()
        result.Id <- source.id
        result.Token <- source.token
        result.CreatedOn <- source.createdOn
        result.ExpiresOn <- source.expiresOn
        result.User <- source.user |> toUserDto
        result