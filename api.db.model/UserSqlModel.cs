using System;
using System.Collections.Generic;

namespace Apex.Api.Db.Model
{
    public class UserSqlModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte FailedLoginAttempts { get; set; }
        public DateTime? LastFailedLoginAttemptOn { get; set; }
        public List<PrivilegeSqlModel> Privileges { get; set; }
    }
}
