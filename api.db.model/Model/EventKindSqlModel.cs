using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("EventKinds")]
    public class EventKindSqlModel
    {
        [Key]
        [Column("EventKindId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
