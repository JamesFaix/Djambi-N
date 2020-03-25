namespace Apex.Api.Db.Model
{
    public class PlayerSqlModel
    {
        public int Id { get; set; }
        public GameSqlModel Game { get; set; }
        public UserSqlModel User { get; set; }
        public PlayerKindSqlModel Kind { get; set; }
        public string Name { get; set; }
        public PlayerStatusSqlModel Status { get; set; }
        public byte? ColorId { get; set; }
        public byte? StartingRegion { get; set; }
        public byte? StartingTurnNumber { get; set; }
    }
}
