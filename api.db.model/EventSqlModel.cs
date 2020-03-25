using System;

namespace Apex.Api.Db.Model
{
    public class EventSqlModel
    {
        public int Id { get; set; }
        public GameSqlModel Game { get; set; }
        public UserSqlModel CreatedByUser { get; set; }
        public PlayerSqlModel ActingPlayer { get; set; }
        public DateTime CreatedOn { get; set; }
        public EventKindSqlModel Kind { get; set; }
        public string EffectsJson { get; set; }
    }
}
