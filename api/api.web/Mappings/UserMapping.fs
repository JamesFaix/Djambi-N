namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module UserMapping =
    
    let toCreateUserRequest (source : CreateUserRequestDto) : CreateUserRequest =
        {
            name = source.name
            password = source.password
        }

    let toUserDto (source : User) : UserDto =
        {
            id = source.id
            name = source.name
            privileges = source.privileges
        }

    let toCreationSourceDto (source : CreationSource) : CreationSourceDto =
        {
            userId = source.userId
            userName = source.userName
            time = source.time
        }