using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("UserPrivileges")]
    public class UserPrivilegeSqlModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public UserSqlModel User { get; set; }

        [Required]
        public byte PrivilegeId { get; set; }
        public PrivilegeSqlModel Privilege { get; set; }
    }
}
