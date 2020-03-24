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

    let toPrivilegeDto (source : Privilege) : PrivilegeDto =
        match source with
        | Privilege.EditPendingGames -> PrivilegeDto.EditPendingGames
        | Privilege.EditUsers -> PrivilegeDto.EditUsers
        | Privilege.OpenParticipation -> PrivilegeDto.OpenParticipation
        | Privilege.Snapshots -> PrivilegeDto.Snapshots
        | Privilege.ViewGames -> PrivilegeDto.ViewGames

    let toUserDto (source : User) : UserDto =
        {
            id = source.id
            name = source.name
            privileges = source.privileges |> List.map toPrivilegeDto
        }

    let toCreationSourceDto (source : CreationSource) : CreationSourceDto =
        {
            userId = source.userId
            userName = source.userName
            time = source.time
        }