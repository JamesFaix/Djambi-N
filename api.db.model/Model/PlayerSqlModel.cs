using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Players")]
    public class PlayerSqlModel
    {
        [Key]
        [Required]
        public int PlayerId { get; set; }

        [Required]
        public int GameId { get; set; }
        public GameSqlModel Game { get; set; }

        public int? UserId { get; set; }
        public UserSqlModel User { get; set; }

        [Required]
        public byte PlayerKindId { get; set; }
        public PlayerKindSqlModel PlayerKind { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public byte PlayerStatusId { get; set; }
        public PlayerStatusSqlModel PlayerStatus { get; set; }

        public byte? ColorId { get; set; }

        public byte? StartingRegion { get; set; }

        public byte? StartingTurnNumber { get; set; }
    }
}
