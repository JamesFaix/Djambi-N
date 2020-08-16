using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Djambi.Api.Enums;

namespace Djambi.Api.Db.Model
{
    [Table("Privileges")]
    public class PrivilegeSqlModel
    {
        [Key]
        [Column("PrivilegeId")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Privilege Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
