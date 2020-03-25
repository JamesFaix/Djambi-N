namespace Apex.Api.Db.Model
{
    public class EventKindSqlModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GameStatusSqlModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PlayerKindSqlModel
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class PlayerStatusSqlModel
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class PrivilegeSqlModel
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }
}
