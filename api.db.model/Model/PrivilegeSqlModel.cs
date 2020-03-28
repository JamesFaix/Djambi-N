using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Privileges")]
    public class PrivilegeSqlModel
    {
        [Key]
        [Column("PrivilegeId")]
        [Required]
        public byte Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
