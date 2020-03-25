using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Apex.Api.Db.Model
{
    public class UserSqlModel
    {
        [Required]
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; }

        [Required] 
        public string Password { get; set; }

        [Required] 
        public DateTime CreatedOn { get; set; }

        [Required] 
        public byte FailedLoginAttempts { get; set; }

        public DateTime? LastFailedLoginAttemptOn { get; set; }

        [Required] 
        public List<PrivilegeSqlModel> Privileges { get; set; }
    }
}
