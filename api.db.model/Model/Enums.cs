using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("EventKinds")]
    public class EventKindSqlModel
    {
        [Required]
        public byte Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    [Table("GameStatuses")]
    public class GameStatusSqlModel
    {
        [Required]
        public byte Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    [Table("PlayerKinds")]
    public class PlayerKindSqlModel
    {
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    [Table("PlayerStatuses")]
    public class PlayerStatusSqlModel
    {
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    [Table("Privileges")]
    public class PrivilegeSqlModel
    {
        [Required]
        public byte Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
