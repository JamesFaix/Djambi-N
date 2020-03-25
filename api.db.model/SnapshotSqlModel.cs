using System;

namespace Apex.Api.Db.Model
{
    public class SnapshotSqlModel
    {
        public int Id { get; set; }
        public GameSqlModel Game { get; set; }
        public UserSqlModel CreatedByUser { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public string SnapshotJson { get; set; }
    }
}
