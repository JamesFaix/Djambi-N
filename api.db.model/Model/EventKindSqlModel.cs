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
        public EventKindSqlId Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }

    public enum EventKindSqlId : byte
    {
        GameParametersChanged = 1,
        GameCanceled = 2,
        PlayerJoined = 3,
        PlayerRemoved = 4,
        GameStarted = 5,
        TurnCommitted = 6,
        TurnReset = 7,
        CellSelected = 8,
        PlayerStatusChanged = 9
    }
}
