using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Snapshots")]
    public class SnapshotSqlModel
    {
        [Key]
        [Required]
        public int SnapshotId { get; set; }

        [Required]
        public int GameId { get; set; }
        public GameSqlModel Game { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        public UserSqlModel CreatedByUser { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        public string SnapshotJson { get; set; }
    }
}
