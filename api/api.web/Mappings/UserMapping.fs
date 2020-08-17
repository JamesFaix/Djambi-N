namespace Djambi.Api.Web.Mappings

open Djambi.Api.Model
open Djambi.Api.Web.Model

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