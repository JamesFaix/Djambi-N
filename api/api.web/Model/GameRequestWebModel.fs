namespace Apex.Api.Web.Model

open System
open System.ComponentModel

[<CLIMutable>]
type CreatePlayerRequestDto = {
    kind : PlayerKindDto
    userId : Nullable<int>

    // Nullable
    name : string
}

[<CLIMutable>]
type SelectionRequestDto = {
    cellId : int
}

[<CLIMutable>]
type EventsQueryDto = {
    maxResults : Nullable<int>
    direction : ListSortDirection
    thresholdTime : Nullable<DateTime>
    thresholdEventId : Nullable<int>
}