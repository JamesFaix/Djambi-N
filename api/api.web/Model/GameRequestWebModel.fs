namespace Apex.Api.Web.Model

open System
open System.ComponentModel
open Apex.Api.Enums

[<CLIMutable>]
type CreatePlayerRequestDto = {
    kind : PlayerKind
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