using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("GameStatuses")]
    public class GameStatusSqlModel
    {
        [Key]
        [Column("GameStatusId")]
        [Required]
        public GameStatusSqlId Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    public enum GameStatusSqlId : byte
    {
        Canceled = 1,
        InProgress = 2,
        Over = 3,
        Pending = 4
    }
}
