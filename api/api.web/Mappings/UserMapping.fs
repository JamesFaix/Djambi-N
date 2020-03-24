namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module UserMapping =
    
    let toCreateUserRequest (source : CreateUserRequestDto) : CreateUserRequest =
        {
            name = source.Name
            password = source.Password
        }

    let toPrivilegeDto (source : Privilege) : PrivilegeDto =
        match source with
        | Privilege.EditPendingGames -> PrivilegeDto.EditPendingGames
        | Privilege.EditUsers -> PrivilegeDto.EditUsers
        | Privilege.OpenParticipation -> PrivilegeDto.OpenParticipation
        | Privilege.Snapshots -> PrivilegeDto.Snapshots
        | Privilege.ViewGames -> PrivilegeDto.ViewGames

    let toUserDto (source : User) : UserDto =
        let result = UserDto()
        result.Id <- source.id
        result.Name <- source.name
        result.Privileges <- 
            source.privileges 
            |> List.map toPrivilegeDto
            |> List.toArray
        result

    let toCreationSourceDto (source : CreationSource) : CreationSourceDto =
        let result = CreationSourceDto()
        result.UserId <- source.userId
        result.UserName <-source.userName
        result.Time <- source.time
        result