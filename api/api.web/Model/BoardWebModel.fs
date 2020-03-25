namespace Apex.Api.Web.Model

open System.ComponentModel.DataAnnotations

type LocationDto = {
    region : int
    x : int
    y : int
}

type CellDto = {
    id : int

    [<Required>]
    locations : List<LocationDto>
}

type BoardDto = {
    regionCount : int

    regionSize : int
    
    [<Required>]
    cells : List<CellDto>
}