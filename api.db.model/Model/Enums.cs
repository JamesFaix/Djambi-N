using System.ComponentModel.DataAnnotations;

namespace Apex.Api.Db.Model
{
    public class EventKindSqlModel
    {
        [Required]
        public int Id { get; set; }
        
        [Required] 
        public string Name { get; set; }
    }

    public class GameStatusSqlModel
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
    }

    public class PlayerKindSqlModel
    {
        [Required]
        public byte Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class PlayerStatusSqlModel
    {
        [Required]
        public byte Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class PrivilegeSqlModel
    {
        [Required]
        public byte Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
