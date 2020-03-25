using System.ComponentModel.DataAnnotations;

namespace Apex.Api.Db.Model
{
    public class PlayerSqlModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public GameSqlModel Game { get; set; }

        // Nullable
        public UserSqlModel User { get; set; }

        [Required]
        public PlayerKindSqlModel Kind { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public PlayerStatusSqlModel Status { get; set; }

        public byte? ColorId { get; set; }

        public byte? StartingRegion { get; set; }

        public byte? StartingTurnNumber { get; set; }
    }
}
