using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Snapshots")]
    public class SnapshotSqlModel
    {
        [Required]
        public int Id { get; set; }

        [Required] 
        public GameSqlModel Game { get; set; }

        [Required] 
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
