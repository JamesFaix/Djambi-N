using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Apex.Api.Enums;

namespace Apex.Api.Db.Model
{
    [Table("EventKinds")]
    public class EventKindSqlModel
    {
        [Key]
        [Column("EventKindId")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public EventKind Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
