namespace Apex.Api.Web.Model

open System.ComponentModel.DataAnnotations

type LocationDto = {
    [<Required>]
    region : int
    
    [<Required>]
    x : int
    
    [<Required>]
    y : int
}

type CellDto = {
    [<Required>]
    id : int

    [<Required>]
    locations : List<LocationDto>
}

type BoardDto = {
    [<Required>]
    regionCount : int

    [<Required>]
    regionSize : int
    
    [<Required>]
    cells : List<CellDto>
}