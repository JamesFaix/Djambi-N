//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Apex.Api.Db.Model
//{
//    [Table("Games")]
//    public class GameSqlModel
//    {
//        [Required]
//        public int Id { get; set; }

//        [Required]
//        public int CreatedByUserId { get; set; }
//        public UserSqlModel CreatedByUser { get; set; }

//        [Required]
//        public DateTime CreatedOn { get; set; }

//        [Required]
//        public byte StatusId { get; set; }
//        public GameStatusSqlModel Status { get; set; }

//        public IList<PlayerSqlModel> Players { get; set; }

//        public IList<EventSqlModel> Events { get; set; }

//        public string Description { get; set; }

//        [Required]
//        public byte RegionCount { get; set; }

//        [Required]
//        public bool AllowGuests { get; set; }

//        [Required]
//        public bool IsPublic { get; set; }

//        // Nullable
//        public string TurnCycleJson { get; set; }

//        // Nullable
//        public string PiecesJson { get; set; }

//        // Nullable
//        public string CurrentTurnJson { get; set; }
//    }
//}
