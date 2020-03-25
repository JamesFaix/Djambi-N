using System;
using System.ComponentModel.DataAnnotations;

namespace Apex.Api.Db.Model
{
    public class GameSqlModel
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public UserSqlModel CreatedByUser { get; set; }
        
        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public GameStatusSqlModel Status { get; set; }
        public string Description { get; set; }

        [Required]
        public byte RegionCount { get; set; }

        [Required]
        public bool AllowGuests { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        // Nullable
        public string TurnCycleJson { get; set; }

        // Nullable
        public string PiecesJson { get; set; }
        
        // Nullable
        public string CurrentTurnJson { get; set; }
    }
}
