using System;
using System.ComponentModel;

namespace Apex.Api.Web.Model
{
    public class CreatePlayerRequest
    {
        public PlayerKind Kind { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
    }

    public class SelectionRequest
    {
        public int CellId { get; set; }
    }

    public class EventsQuery
    {
        public int? MaxResults { get; set; }
        public ListSortDirection Direction { get; set; }
        public DateTime? ThresholdTime { get; set; }
        public int? ThresholdEventId { get; set; }
    }
}
