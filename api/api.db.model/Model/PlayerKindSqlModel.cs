using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Djambi.Api.Enums;

namespace Djambi.Api.Db.Model
{
    [Table("PlayerKinds")]
    public class PlayerKindSqlModel
    {
        [Key]
        [Column("PlayerKindId")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public PlayerKind Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
