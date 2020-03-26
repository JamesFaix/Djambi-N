using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Events")]
    public class EventSqlModel
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public int GameId { get; set; }

        [Required]
        public GameSqlModel Game { get; set; }
        
        [Required]
        public int CreatedByUserId { get; set; }

        [Required]
        public UserSqlModel CreatedByUser { get; set; }
        
        public int? ActingPlayerId { get; set; }

        // Nullable
        public PlayerSqlModel ActingPlayer { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
        
        [Required]
        public byte KindId { get; set; }

        [Required]
        public EventKindSqlModel Kind { get; set; }

        [Required]
        public string EffectsJson { get; set; }
    }
}
