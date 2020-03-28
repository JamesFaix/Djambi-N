using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("PlayerStatuses")]
    public class PlayerStatusSqlModel
    {
        [Key]
        [Column("PlayerStatusId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
