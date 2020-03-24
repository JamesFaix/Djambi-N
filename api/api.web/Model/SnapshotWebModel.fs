namespace Apex.Api.Web.Model

type SnapshotInfoDto = {
    id : int
    createdBy : CreationSourceDto
    description : string
}

[<CLIMutable>]
type CreateSnapshotRequestDto = {
    description : string
}