using System;

namespace Apex.Api.Web.Model
{
    public class GamesQuery
    {
        public int? GameId { get; set; }
        public string DescriptionContains { get; set; }
        public string CreatedByUserName { get; set; }
        public string PlayerUserName { get; set; }
        public bool? ContainsMe { get; set; }
        public bool? AllowGuests { get; set; }
        public GameStatus[] Statuses { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? LastEventBefore { get; set; }
        public DateTime? LastEventAfter { get; set; }
    }

    public class SearchGame
    {
        public int Id { get; set; }
        public GameParameters Parameters { get; set; }
        public CreationSource CreatedBy { get; set; }
        public GameStatus Status { get; set; }
        public DateTime LastEventOn { get; set; }
        public int PlayerCount { get; set; }
        public bool ContainsMe { get; set; }
        //TODO: Add current player name
    }
}
