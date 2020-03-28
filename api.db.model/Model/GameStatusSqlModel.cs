using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("GameStatuses")]
    public class GameStatusSqlModel
    {
        [Key]
        [Column("GameStatusId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
