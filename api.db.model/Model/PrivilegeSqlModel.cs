using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Apex.Api.Enums;

namespace Apex.Api.Db.Model
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
