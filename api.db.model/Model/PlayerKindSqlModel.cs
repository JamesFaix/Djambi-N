using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Apex.Api.Enums;

namespace Apex.Api.Db.Model
{
    [Table("PlayerKinds")]
    public class PlayerKindSqlModel
    {
        [Key]
        [Column("PlayerKindId")]
        [Required]
        public PlayerKind Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
