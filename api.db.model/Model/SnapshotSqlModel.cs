using System;
using System.ComponentModel.DataAnnotations;

namespace Apex.Api.Db.Model
{
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
        public string Description { get; set; }

        [Required] 
        public string SnapshotJson { get; set; }
    }
}
