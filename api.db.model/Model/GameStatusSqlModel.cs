using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Apex.Api.Enums;

namespace Apex.Api.Db.Model
{
    [Table("GameStatuses")]
    public class GameStatusSqlModel
    {
        [Key]
        [Column("GameStatusId")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public GameStatus Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
