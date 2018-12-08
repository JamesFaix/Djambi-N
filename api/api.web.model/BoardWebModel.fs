[<AutoOpen>]
module Djambi.Api.WebModel.BoardWebModel

type LocationJsonModel =
    {
        region : int
        x : int
        y : int
    }

type CellJsonModel =
    {
        id : int
        locations : LocationJsonModel list
    }