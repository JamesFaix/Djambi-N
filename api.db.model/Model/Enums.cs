using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("EventKinds")]
    public class EventKindSqlModel
    {
        [Key]
        [Column("EventKindId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    [Table("GameStatuses")]
    public class GameStatusSqlModel
    {
        [Key]
        [Column("GameStatusId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    [Table("PlayerKinds")]
    public class PlayerKindSqlModel
    {
        [Key]
        [Column("PlayerKindId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    [Table("PlayerStatuses")]
    public class PlayerStatusSqlModel
    {
        [Key]
        [Column("PlayerStatusId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    [Table("Privileges")]
    public class PrivilegeSqlModel
    {
        [Key]
        [Column("PrivilegeId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
