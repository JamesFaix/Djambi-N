using System;

namespace Apex.Api.Db.Model
{
    public class GameSqlModel
    {
        public int Id { get; set; }
        public UserSqlModel CreatedByUser { get; set; }
        public DateTime CreatedOn { get; set; }
        public GameStatusSqlModel Status { get; set; }
        public string Description { get; set; }
        public byte RegionCount { get; set; }
        public bool AllowGuests { get; set; }
        public bool IsPublic { get; set; }
        public string TurnCycleJson { get; set; }
        public string PiecesJson { get; set; }
        public string CurrentTurnJson { get; set; }
    }
}
