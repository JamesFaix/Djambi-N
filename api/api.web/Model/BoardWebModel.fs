namespace Apex.Api.Web.Model

type LocationDto = {
    region : int
    x : int
    y : int
}

type CellDto = {
    id : int
    locations : List<LocationDto>
}

type BoardDto = {
    regionCount : int
    regionSize : int
    cells : List<CellDto>
}