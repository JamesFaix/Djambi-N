﻿namespace Djambi.Api.Web.Model

open System.ComponentModel.DataAnnotations

type SnapshotInfoDto = {
    [<Required>]
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