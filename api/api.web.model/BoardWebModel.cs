namespace Apex.Api.Web.Model
{
    public class Location
    {
        public int Region { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Cell
    {
        public int Id { get; set; }
        public Location[] Locations { get; set; }
    }

    public class Board
    {
        public int RegionCount { get; set; }
        public int RegionSize { get; set; }
        public Cell[] Cells { get; set; }
    }
}
