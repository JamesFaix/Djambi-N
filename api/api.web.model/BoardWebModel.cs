namespace Apex.Api.Web.Model
{
    public class LocationDto
    {
        public int Region { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class CellDto
    {
        public int Id { get; set; }
        public LocationDto[] Locations { get; set; }
    }

    public class BoardDto
    {
        public int RegionCount { get; set; }
        public int RegionSize { get; set; }
        public CellDto[] Cells { get; set; }
    }
}
