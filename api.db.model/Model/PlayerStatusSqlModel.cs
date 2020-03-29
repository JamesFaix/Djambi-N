using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Apex.Api.Enums;

namespace Apex.Api.Db.Model
{
    [Table("PlayerStatuses")]
    public class PlayerStatusSqlModel
    {
        [Key]
        [Column("PlayerStatusId")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public PlayerStatus Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
