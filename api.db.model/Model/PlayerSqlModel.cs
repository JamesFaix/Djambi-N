using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Players")]
    public class PlayerSqlModel
    {
        [Required]
        [Column("PlayerId")]
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }
        public GameSqlModel Game { get; set; }

        public int? UserId { get; set; }
        public UserSqlModel User { get; set; }

        [Required]
        public byte KindId { get; set; }
        public PlayerKindSqlModel Kind { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public byte StatusId { get; set; }
        public PlayerStatusSqlModel Status { get; set; }

        public byte? ColorId { get; set; }

        public byte? StartingRegion { get; set; }

        public byte? StartingTurnNumber { get; set; }
    }
}
