using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Djambi.Api.Enums;

namespace Djambi.Api.Db.Model
{
    [Table("Events")]
    public class EventSqlModel
    {
        [Key]
        [Required]
        public int EventId { get; set; }

        [Required]
        public int GameId { get; set; }
        public GameSqlModel Game { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        public UserSqlModel CreatedByUser { get; set; }

        public int? ActingPlayerId { get; set; }
        public PlayerSqlModel ActingPlayer { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public EventKind EventKindId { get; set; }

        [Required]
        public string EffectsJson { get; set; }
    }
}
