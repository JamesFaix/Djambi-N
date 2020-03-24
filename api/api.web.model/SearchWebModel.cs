using System;

namespace Apex.Api.Web.Model
{
    public class GamesQueryDto
    {
        public int? GameId { get; set; }
        public string DescriptionContains { get; set; }
        public string CreatedByUserName { get; set; }
        public string PlayerUserName { get; set; }
        public bool? ContainsMe { get; set; }
        public bool? IsPublic { get; set; }
        public bool? AllowGuests { get; set; }
        public GameStatusDto[] Statuses { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? LastEventBefore { get; set; }
        public DateTime? LastEventAfter { get; set; }
    }

    public class SearchGameDto
    {
        public int Id { get; set; }
        public GameParametersDto Parameters { get; set; }
        public CreationSourceDto CreatedBy { get; set; }
        public GameStatusDto Status { get; set; }
        public DateTime LastEventOn { get; set; }
        public int PlayerCount { get; set; }
        public bool ContainsMe { get; set; }
        //TODO: Add current player name
    }
}
