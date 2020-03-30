using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Users")]
    public class UserSqlModel
    {
        [Key]
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public byte FailedLoginAttempts { get; set; }

        public DateTime? LastFailedLoginAttemptOn { get; set; }

        public List<UserPrivilegeSqlModel> UserPrivileges { get; set; } = new List<UserPrivilegeSqlModel>();
    }
}
