namespace Apex.Api.Web.Model

open System.ComponentModel.DataAnnotations

type SnapshotInfoDto = {
    id : int

    [<Required>]
    createdBy : CreationSourceDto

    [<Required>]
    description : string
}

[<CLIMutable>]
type CreateSnapshotRequestDto = {
    [<Required>]
    [<StringLength(100, MinimumLength = 1)>]
    description : string
}